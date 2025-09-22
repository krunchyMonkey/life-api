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
  const speedRef = useRef<number>(defaultSpeed);
  const lastPerformanceUpdate = useRef<number>(0);
  const pendingGenerationRef = useRef<boolean>(false);
  const consecutiveErrorsRef = useRef<number>(0);

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

  // Performance tracking - throttled and disabled at high speeds to reduce re-renders
  const trackPerformance = useCallback((operation: string, startTime: number, endTime: number) => {
    // Disable performance tracking at speeds higher than 10/sec to reduce re-renders
    if (gameState.speed > 10) return;
    
    // Throttle performance updates to max 3 times per second
    const now = Date.now();
    if (now - lastPerformanceUpdate.current < 333) return;
    
    lastPerformanceUpdate.current = now;
    
    const metrics: PerformanceMetrics = {
      generationTime: endTime - startTime,
      renderTime: 0, // This would be measured in the component
      apiResponseTime: endTime - startTime,
      cellsCount: gameState.board?.aliveCells.length || 0,
      generationsPerSecond: gameState.speed,
      lastUpdate: now
    };
    setPerformanceMetrics(metrics);
    console.log(`⏱️ ${operation}:`, metrics);
  }, [gameState.board, gameState.speed]);

  // Set board - use functional update to avoid dependency on gameState.board
  const setBoard = useCallback((board: BoardDto) => {
    setGameState(prev => {
      // Add current board to history if it exists
      if (prev.board) {
        const newHistory = [...prev.history, prev.board].slice(-maxHistory);
        historyIndexRef.current = -1;
        redoStackRef.current = [];
        return {
          ...prev,
          board,
          history: newHistory
        };
      }
      return { ...prev, board };
    });
    setError(null);
  }, [maxHistory]);

  // Get next generation - simplified with proper async handling
  const nextGeneration = useCallback(async () => {
    if (!gameState.board || pendingGenerationRef.current) return;

    const startTime = performance.now();
    pendingGenerationRef.current = true;
    
    try {
      const nextPositions = await nextGenerationMutation.mutateAsync(gameState.board);
      const nextBoard = positionsToBoard(nextPositions, gameState.board.width, gameState.board.height, (gameState.board.generation || 0) + 1);
      
      // Reset error counter on success
      consecutiveErrorsRef.current = 0;
      setBoard(nextBoard);
      
      const endTime = performance.now();
      trackPerformance('Next Generation', startTime, endTime);
    } catch (err: any) {
      console.error('❌ Next generation API error:', err);
      consecutiveErrorsRef.current += 1;
      
      // Handle database concurrency errors gracefully
      if (err.message?.includes('DbUpdateException') || 
          err.message?.includes('EntityFrameworkCore') ||
          err.message?.includes('entity changes')) {
        console.warn('🔄 Database concurrency issue, skipping this generation');
        
        // If we get too many consecutive DB errors, pause the game
        if (consecutiveErrorsRef.current >= 3) {
          console.warn('⏸️ Too many consecutive DB errors, pausing game');
          setGameState(prev => ({ ...prev, isPlaying: false }));
          setError('Game paused due to database errors. Try reducing speed or restarting.');
        }
      } else {
        // For other errors, show them to the user and pause
        setGameState(prev => ({ ...prev, isPlaying: false }));
        setError(err instanceof Error ? err.message : 'Failed to get next generation');
      }
    } finally {
      pendingGenerationRef.current = false;
    }
  }, [gameState.board, nextGenerationMutation, setBoard, trackPerformance, positionsToBoard]);

  // Advance multiple generations - simplified
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

  // Get final state - simplified
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

  // Update speed ref whenever speed changes
  useEffect(() => {
    speedRef.current = gameState.speed;
  }, [gameState.speed]);

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

  // Game loop effect - manages the interval for automatic generation progression
  useEffect(() => {
    if (gameState.isPlaying && !pendingGenerationRef.current) {
      const interval = Math.max(100, 1000 / gameState.speed); // Minimum 100ms between calls
      
      intervalRef.current = window.setInterval(() => {
        // Only trigger next generation if there's no pending API call
        if (!pendingGenerationRef.current) {
          nextGeneration();
        }
      }, interval);
    } else {
      // Clear interval when not playing
      if (intervalRef.current) {
        window.clearInterval(intervalRef.current);
        intervalRef.current = null;
      }
    }

    // Cleanup on unmount or when dependencies change
    return () => {
      if (intervalRef.current) {
        window.clearInterval(intervalRef.current);
        intervalRef.current = null;
      }
    };
  }, [gameState.isPlaying, gameState.speed, nextGeneration]);

  // Play/pause controls - simplified
  const play = useCallback(() => {
    setGameState(prev => ({ ...prev, isPlaying: true }));
  }, []);

  const pause = useCallback(() => {
    if (intervalRef.current) {
      window.clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
    setGameState(prev => ({ ...prev, isPlaying: false }));
  }, []);

  // Reset to first generation - simplified
  const reset = useCallback(() => {
    setGameState(prev => ({ ...prev, isPlaying: false }));
    if (gameState.history.length > 0) {
      setGameState(prev => ({ 
        ...prev, 
        board: prev.history[0] || null,
        history: []
      }));
    }
    historyIndexRef.current = -1;
    redoStackRef.current = [];
  }, [gameState.history]);

  // Clear board - simplified
  const clear = useCallback(() => {
    setGameState(prev => ({ ...prev, isPlaying: false }));
    if (gameState.board) {
      const clearedBoard: BoardDto = {
        ...gameState.board,
        aliveCells: [],
        generation: 0
      };
      setBoard(clearedBoard);
    }
  }, [gameState.board, setBoard]);

  // Set speed - simplified
  const setSpeed = useCallback((speed: number) => {
    setGameState(prev => ({ ...prev, speed }));
  }, []);

  // Undo/redo - simplified
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

  // Toggle individual cell - simplified
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

  // Randomize board - simplified  
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