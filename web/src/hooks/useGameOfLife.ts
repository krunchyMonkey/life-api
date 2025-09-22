import { useState, useCallback, useRef, useEffect } from 'react';
import { useNextGeneration, useAdvanceGenerations, useFinalState } from './useApi';
import type { BoardDto, GameState, PerformanceMetrics, Position } from '../types/api';

interface UseGameOfLifeOptions {
  maxHistory?: number;
  defaultSpeed?: number;
  autoSave?: boolean;
}

interface UseGameOfLifeReturn {
  // State
  gameState: GameState;
  isLoading: boolean;
  error: string | null;
  performanceMetrics: PerformanceMetrics | null;
  
  // Actions
  setBoard: (board: BoardDto) => void;
  nextGeneration: () => void;
  advanceGenerations: (n: number) => void;
  getFinalState: () => void;
  play: () => void;
  pause: () => void;
  reset: () => void;
  clear: () => void;
  setSpeed: (speed: number) => void;
  
  // Utilities
  canUndo: boolean;
  canRedo: boolean;
  undo: () => void;
  redo: () => void;
  toggleCell: (x: number, y: number) => void;
  randomizeBoard: (density?: number) => void;
}

export const useGameOfLife = (options: UseGameOfLifeOptions = {}): UseGameOfLifeReturn => {
  const {
    maxHistory = 50,
    defaultSpeed = 5,
    autoSave = true
  } = options;

  // Helper function to convert positions to BoardDto
  const positionsToBoard = useCallback((positions: Position[], width: number, height: number, generation: number = 0): BoardDto => {
    return {
      width,
      height,
      aliveCells: positions,
      generation
    };
  }, []);

  // API hooks
  const nextGenerationMutation = useNextGeneration();
  const advanceGenerationsMutation = useAdvanceGenerations();
  const finalStateMutation = useFinalState();

  // Game state
  const [gameState, setGameState] = useState<GameState>({
    board: null,
    isPlaying: false,
    speed: defaultSpeed,
    history: [],
    maxHistory
  });

  // UI state
  const [error, setError] = useState<string | null>(null);
  const [performanceMetrics, setPerformanceMetrics] = useState<PerformanceMetrics | null>(null);

  // Animation state
  const intervalRef = useRef<number | null>(null);
  const historyIndexRef = useRef<number>(-1);
  const redoStackRef = useRef<BoardDto[]>([]);

  // Computed state
  const isLoading = nextGenerationMutation.isPending || 
                   advanceGenerationsMutation.isPending || 
                   finalStateMutation.isPending;

  const canUndo = gameState.history.length > 0 && historyIndexRef.current < gameState.history.length - 1;
  const canRedo = redoStackRef.current.length > 0;

  // Clear any running intervals on unmount
  useEffect(() => {
    return () => {
      if (intervalRef.current) {
        window.clearInterval(intervalRef.current);
      }
    };
  }, []);

  // Auto-save to localStorage
  useEffect(() => {
    if (autoSave && gameState.board) {
      localStorage.setItem('gameOfLife_currentBoard', JSON.stringify(gameState.board));
      localStorage.setItem('gameOfLife_settings', JSON.stringify({
        speed: gameState.speed,
        isPlaying: gameState.isPlaying
      }));
    }
  }, [gameState.board, gameState.speed, gameState.isPlaying, autoSave]);

  // Performance tracking
  const trackPerformance = useCallback((operation: string, startTime: number, endTime: number) => {
    const metrics: PerformanceMetrics = {
      generationTime: endTime - startTime,
      renderTime: 0, // This would be measured in the component
      apiResponseTime: endTime - startTime,
      cellsCount: gameState.board?.aliveCells.length || 0,
      generationsPerSecond: gameState.speed
    };
    setPerformanceMetrics(metrics);
    console.log(`⏱️ ${operation}:`, metrics);
  }, [gameState.board, gameState.speed]);

  // Add board to history
  const addToHistory = useCallback((board: BoardDto) => {
    setGameState(prev => {
      const newHistory = [...prev.history, board].slice(-maxHistory);
      return {
        ...prev,
        history: newHistory
      };
    });
    historyIndexRef.current = -1;
    redoStackRef.current = [];
  }, [maxHistory]);

  // Set board
  const setBoard = useCallback((board: BoardDto) => {
    if (gameState.board) {
      addToHistory(gameState.board);
    }
    setGameState(prev => ({ ...prev, board }));
    setError(null);
  }, [gameState.board, addToHistory]);

  // Get next generation
  const nextGeneration = useCallback(async () => {
    if (!gameState.board) return;

    const startTime = performance.now();
    
    try {
      const nextPositions = await nextGenerationMutation.mutateAsync(gameState.board);
      const nextBoard = positionsToBoard(nextPositions, gameState.board.width, gameState.board.height, (gameState.board.generation || 0) + 1);
      setBoard(nextBoard);
      
      const endTime = performance.now();
      trackPerformance('Next Generation', startTime, endTime);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to get next generation');
    }
  }, [gameState.board, nextGenerationMutation, setBoard, trackPerformance, positionsToBoard]);

  // Advance multiple generations
  const advanceGenerations = useCallback(async (n: number) => {
    if (!gameState.board || n <= 0) return;

    const startTime = performance.now();
    
    try {
      const advancedPositions = await advanceGenerationsMutation.mutateAsync({
        board: gameState.board,
        n
      });
      const advancedBoard = positionsToBoard(advancedPositions, gameState.board.width, gameState.board.height, (gameState.board.generation || 0) + n);
      setBoard(advancedBoard);
      
      const endTime = performance.now();
      trackPerformance(`Advance ${n} Generations`, startTime, endTime);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to advance generations');
    }
  }, [gameState.board, advanceGenerationsMutation, setBoard, trackPerformance, positionsToBoard]);

  // Get final state
  const getFinalState = useCallback(async () => {
    if (!gameState.board) return;

    const startTime = performance.now();
    
    try {
      const finalResult = await finalStateMutation.mutateAsync(gameState.board);
      const finalBoard = positionsToBoard(finalResult.positions, gameState.board.width, gameState.board.height, finalResult.iterations);
      setBoard(finalBoard);
      
      const endTime = performance.now();
      trackPerformance('Get Final State', startTime, endTime);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to get final state');
    }
  }, [gameState.board, finalStateMutation, setBoard, trackPerformance, positionsToBoard]);

  // Play/pause controls
  const play = useCallback(() => {
    if (intervalRef.current) return; // Already playing

    setGameState(prev => ({ ...prev, isPlaying: true }));
    
    intervalRef.current = window.setInterval(() => {
      nextGeneration();
    }, 1000 / gameState.speed);
  }, [nextGeneration, gameState.speed]);

  const pause = useCallback(() => {
    if (intervalRef.current) {
      window.clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
    setGameState(prev => ({ ...prev, isPlaying: false }));
  }, []);

  // Reset to first generation
  const reset = useCallback(() => {
    pause();
    if (gameState.history.length > 0) {
      setGameState(prev => ({ 
        ...prev, 
        board: prev.history[0] || null,
        history: []
      }));
    }
    historyIndexRef.current = -1;
    redoStackRef.current = [];
  }, [pause, gameState.history]);

  // Clear board
  const clear = useCallback(() => {
    pause();
    if (gameState.board) {
      const clearedBoard: BoardDto = {
        ...gameState.board,
        aliveCells: [],
        generation: 0
      };
      setBoard(clearedBoard);
    }
  }, [pause, gameState.board, setBoard]);

  // Set speed
  const setSpeed = useCallback((speed: number) => {
    setGameState(prev => ({ ...prev, speed }));
    
    // If playing, restart with new speed
    if (gameState.isPlaying) {
      pause();
      setTimeout(() => play(), 100);
    }
  }, [gameState.isPlaying, pause, play]);

  // Undo/redo
  const undo = useCallback(() => {
    if (!canUndo) return;

    const historyIndex = historyIndexRef.current + 1;
    const previousBoard = gameState.history[gameState.history.length - 1 - historyIndex];
    
    if (previousBoard && gameState.board) {
      redoStackRef.current.push(gameState.board);
      setGameState(prev => ({ ...prev, board: previousBoard }));
      historyIndexRef.current = historyIndex;
    }
  }, [canUndo, gameState.history, gameState.board]);

  const redo = useCallback(() => {
    if (!canRedo) return;

    const redoBoard = redoStackRef.current.pop();
    if (redoBoard) {
      setGameState(prev => ({ ...prev, board: redoBoard }));
      historyIndexRef.current = Math.max(-1, historyIndexRef.current - 1);
    }
  }, [canRedo]);

  // Toggle individual cell
  const toggleCell = useCallback((x: number, y: number) => {
    if (!gameState.board) return;

    const existingCell = gameState.board.aliveCells.find(cell => cell.x === x && cell.y === y);
    
    const newAliveCells = existingCell
      ? gameState.board.aliveCells.filter(cell => !(cell.x === x && cell.y === y))
      : [...gameState.board.aliveCells, { x, y }];

    const newBoard: BoardDto = {
      ...gameState.board,
      aliveCells: newAliveCells
    };

    setBoard(newBoard);
  }, [gameState.board, setBoard]);

  // Randomize board
  const randomizeBoard = useCallback((density = 0.3) => {
    if (!gameState.board) return;

    const { width, height } = gameState.board;
    const newAliveCells = [];

    for (let x = 0; x < width; x++) {
      for (let y = 0; y < height; y++) {
        if (Math.random() < density) {
          newAliveCells.push({ x, y });
        }
      }
    }

    const randomBoard: BoardDto = {
      ...gameState.board,
      aliveCells: newAliveCells,
      generation: 0
    };

    setBoard(randomBoard);
  }, [gameState.board, setBoard]);

  return {
    // State
    gameState,
    isLoading,
    error,
    performanceMetrics,
    
    // Actions
    setBoard,
    nextGeneration,
    advanceGenerations,
    getFinalState,
    play,
    pause,
    reset,
    clear,
    setSpeed,
    
    // Utilities
    canUndo,
    canRedo,
    undo,
    redo,
    toggleCell,
    randomizeBoard,
  };
};