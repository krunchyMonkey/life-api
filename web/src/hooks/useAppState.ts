import { useState, useCallback } from 'react';
import type { ConwayPattern } from '../types/api';
import { useGameOfLife } from './useGameOfLife';
import { useApiHealth } from './useApi';
import { useKeyboardShortcuts } from './useKeyboardShortcuts';
import { useFileOperations } from './useFileOperations';
import { useBoardInitialization } from './useBoardInitialization';
import { usePatternOperations } from './usePatternOperations';
import { GAME_CONFIG } from '../constants/appConstants';

/**
 * Master hook that combines all app-level state and operations
 * This could be used as an alternative to having multiple hooks in the component
 */
export const useAppState = () => {
  const [selectedPattern, setSelectedPattern] = useState<ConwayPattern | null>(null);
  const [showPatternLibrary, setShowPatternLibrary] = useState(false);
  
  // Core hooks
  const { data: isApiHealthy, error: apiError } = useApiHealth();
  const game = useGameOfLife(GAME_CONFIG);

  // Operations hooks
  const fileOps = useFileOperations({
    currentBoard: game.gameState.board,
    onBoardLoad: game.setBoard
  });

  const patternOps = usePatternOperations({
    currentBoard: game.gameState.board,
    onBoardUpdate: game.setBoard,
    onPatternLibraryClose: () => setShowPatternLibrary(false)
  });

  // Initialize board from URL or localStorage
  useBoardInitialization({
    onBoardLoad: game.setBoard
  });

  // Keyboard shortcuts
  useKeyboardShortcuts({
    isPlaying: game.gameState.isPlaying,
    isLoading: game.isLoading,
    onPlayPause: () => game.gameState.isPlaying ? game.pause() : game.play(),
    onNextGeneration: game.nextGeneration,
    onRandomize: game.randomizeBoard,
    onClear: game.clear,
    onUndo: game.undo,
    onRedo: game.redo
  });

  // UI handlers
  const handlePatternSelect = useCallback((pattern: ConwayPattern) => {
    setSelectedPattern(pattern);
  }, []);

  const handleTogglePatternLibrary = useCallback(() => {
    setShowPatternLibrary(!showPatternLibrary);
  }, [showPatternLibrary]);

  const handleClosePatternLibrary = useCallback(() => {
    setShowPatternLibrary(false);
  }, []);

  const handleCloseSelectedPattern = useCallback(() => {
    setSelectedPattern(null);
  }, []);

  return {
    // State
    selectedPattern,
    showPatternLibrary,
    isApiHealthy,
    apiError,
    
    // Game state and operations
    game,
    
    // File operations
    fileOps,
    
    // Pattern operations
    patternOps,
    
    // UI handlers
    handlePatternSelect,
    handleTogglePatternLibrary,
    handleClosePatternLibrary,
    handleCloseSelectedPattern,
  };
};