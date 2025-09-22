import React, { useState, useCallback } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import GameBoard from './components/GameBoard';
import { GameControls } from './components/GameControls';
import { AppHeader } from './components/AppHeader';
import { ApiErrorBanner } from './components/ApiErrorBanner';
import { PerformanceMetrics } from './components/PerformanceMetrics';
import { ErrorPanel } from './components/ErrorPanel';
import { PatternLibraryModal } from './components/PatternLibraryModal';
import { SelectedPatternInfo } from './components/SelectedPatternInfo';
import { useGameOfLife } from './hooks/useGameOfLife';
import { useApiHealth } from './hooks/useApi';
import { useKeyboardShortcuts } from './hooks/useKeyboardShortcuts';
import { useFileOperations } from './hooks/useFileOperations';
import { useBoardInitialization } from './hooks/useBoardInitialization';
import { usePatternOperations } from './hooks/usePatternOperations';
import type { ConwayPattern } from './types/api';
import { QUERY_CLIENT_CONFIG, GAME_CONFIG } from './constants/appConstants';

// Create a client
const queryClient = new QueryClient(QUERY_CLIENT_CONFIG);

const AppContent: React.FC = () => {
  const [selectedPattern, setSelectedPattern] = useState<ConwayPattern | null>(null);
  const [showPatternLibrary, setShowPatternLibrary] = useState(false);
  
  // API health check
  const { data: isApiHealthy, error: apiError } = useApiHealth();
  
  // Game of Life hook
  const game = useGameOfLife(GAME_CONFIG);

  // Initialize board on first render only
  useBoardInitialization({
    onBoardLoad: game.setBoard
  });

  // File operations hook
  const fileOps = useFileOperations({
    currentBoard: game.gameState.board,
    onBoardLoad: game.setBoard
  });

  // Pattern operations hook
  const patternOps = usePatternOperations({
    currentBoard: game.gameState.board,
    onBoardUpdate: game.setBoard,
    onPatternLibraryClose: () => setShowPatternLibrary(false)
  });

  // Keyboard shortcuts - simplified
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

  // Handle pattern selection from library - now just for UI state
  const handlePatternSelect = useCallback((pattern: ConwayPattern) => {
    setSelectedPattern(pattern);
  }, []);

  return (
    <div className="min-h-screen bg-gray-50">
      <AppHeader
        isApiHealthy={isApiHealthy}
        onImport={fileOps.handleImport}
        onExport={fileOps.handleExport}
        onShare={fileOps.handleShare}
        onTogglePatternLibrary={() => setShowPatternLibrary(!showPatternLibrary)}
        showPatternLibrary={showPatternLibrary}
        hasBoard={game.gameState.board !== null}
      />

      <ApiErrorBanner error={apiError} />

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
          {/* Game Board - Takes up most of the space */}
          <div className="lg:col-span-3">
            <GameBoard
              board={game.gameState.board}
              onCellClick={game.toggleCell}
              showGrid={true}
              animateChanges={true}
              className="w-full h-96 lg:h-[600px]"
            />
          </div>

          {/* Controls Sidebar */}
          <div className="space-y-6">
            <GameControls
              gameState={game.gameState}
              isLoading={game.isLoading}
              onPlay={game.play}
              onPause={game.pause}
              onNextGeneration={game.nextGeneration}
              onAdvanceGenerations={game.advanceGenerations}
              onGetFinalState={game.getFinalState}
              onReset={game.reset}
              onClear={game.clear}
              onRandomize={game.randomizeBoard}
              onSpeedChange={game.setSpeed}
              onUndo={game.undo}
              onRedo={game.redo}
              canUndo={game.canUndo}
              canRedo={game.canRedo}
            />

            <PerformanceMetrics performanceMetrics={game.performanceMetrics} />
            <ErrorPanel error={game.error} />
          </div>
        </div>
      </main>

      <PatternLibraryModal
        isOpen={showPatternLibrary}
        onClose={() => setShowPatternLibrary(false)}
        onPatternSelect={handlePatternSelect}
        onInsertPattern={patternOps.handlePatternInsert}
        currentBoard={game.gameState.board}
      />

      <SelectedPatternInfo
        pattern={selectedPattern}
        onClose={() => setSelectedPattern(null)}
      />
    </div>
  );
};

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AppContent />
    </QueryClientProvider>
  );
}

export default App;
