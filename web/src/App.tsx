import React, { useState, useEffect, useCallback } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import GameBoard from './components/GameBoard';
import { GameControls } from './components/GameControls';
import { PatternLibrary } from './components/PatternLibrary';
import { useGameOfLife } from './hooks/useGameOfLife';
import { useApiHealth } from './hooks/useApi';
import type { BoardDto, ConwayPattern, Position } from './types/api';
import { Upload, Download, Share2, Activity, AlertCircle } from 'lucide-react';

// Create a client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
    },
  },
});

// Default board for initialization
const createDefaultBoard = (): BoardDto => ({
  width: 50,
  height: 30,
  aliveCells: [
    // Simple glider
    { x: 1, y: 2 },
    { x: 2, y: 3 },
    { x: 3, y: 1 },
    { x: 3, y: 2 },
    { x: 3, y: 3 }
  ],
  generation: 0
});

const AppContent: React.FC = () => {
  const [selectedPattern, setSelectedPattern] = useState<ConwayPattern | null>(null);
  const [showPatternLibrary, setShowPatternLibrary] = useState(false);
  
  // API health check
  const { data: isApiHealthy, error: apiError } = useApiHealth();
  
  // Game of Life hook
  const game = useGameOfLife({
    maxHistory: 100,
    defaultSpeed: 5,
    autoSave: true
  });

  // Initialize with default board
  useEffect(() => {
    // Try to load from localStorage first
    const savedBoard = localStorage.getItem('gameOfLife_currentBoard');
    if (savedBoard) {
      try {
        const board = JSON.parse(savedBoard) as BoardDto;
        game.setBoard(board);
      } catch (error) {
        console.warn('Failed to load saved board:', error);
        game.setBoard(createDefaultBoard());
      }
    } else {
      game.setBoard(createDefaultBoard());
    }
  }, []);

  // Keyboard shortcuts
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      // Only handle shortcuts when not typing in an input
      if (e.target instanceof HTMLInputElement || e.target instanceof HTMLTextAreaElement) {
        return;
      }

      switch (e.key) {
        case ' ':
          e.preventDefault();
          game.gameState.isPlaying ? game.pause() : game.play();
          break;
        case 'ArrowRight':
          if (!game.gameState.isPlaying && !game.isLoading) {
            e.preventDefault();
            game.nextGeneration();
          }
          break;
        case 'r':
        case 'R':
          if (!game.isLoading) {
            e.preventDefault();
            game.randomizeBoard();
          }
          break;
        case 'c':
        case 'C':
          if (!game.isLoading) {
            e.preventDefault();
            game.clear();
          }
          break;
        case 'z':
          if (e.ctrlKey && !game.isLoading) {
            e.preventDefault();
            game.undo();
          }
          break;
        case 'y':
          if (e.ctrlKey && !game.isLoading) {
            e.preventDefault();
            game.redo();
          }
          break;
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [game]);

  // Handle pattern selection from library
  const handlePatternSelect = useCallback((pattern: ConwayPattern) => {
    setSelectedPattern(pattern);
    // Optionally close the pattern library
    // setShowPatternLibrary(false);
  }, []);

  // Handle pattern insertion
  const handlePatternInsert = useCallback((positions: Position[]) => {
    if (!game.gameState.board) return;

    // Add pattern positions to current alive cells
    const existingCells = new Set(
      game.gameState.board.aliveCells.map(cell => `${cell.x},${cell.y}`)
    );

    const newAliveCells = [...game.gameState.board.aliveCells];
    
    positions.forEach(pos => {
      const key = `${pos.x},${pos.y}`;
      if (!existingCells.has(key) && 
          pos.x >= 0 && pos.x < game.gameState.board!.width &&
          pos.y >= 0 && pos.y < game.gameState.board!.height) {
        newAliveCells.push(pos);
      }
    });

    const updatedBoard: BoardDto = {
      ...game.gameState.board,
      aliveCells: newAliveCells
    };

    game.setBoard(updatedBoard);
    setShowPatternLibrary(false);
  }, [game]);

  // File operations
  const handleExport = useCallback(() => {
    if (!game.gameState.board) return;

    const exportData = {
      board: game.gameState.board,
      metadata: {
        exported: new Date().toISOString(),
        version: '1.0',
        generator: 'Conway Game of Life Web App'
      }
    };

    const dataStr = JSON.stringify(exportData, null, 2);
    const dataBlob = new Blob([dataStr], { type: 'application/json' });
    const url = URL.createObjectURL(dataBlob);
    
    const link = document.createElement('a');
    link.href = url;
    link.download = `game-of-life-gen${game.gameState.board.generation}-${Date.now()}.json`;
    link.click();
    
    URL.revokeObjectURL(url);
  }, [game.gameState.board]);

  const handleImport = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (e) => {
      try {
        const data = JSON.parse(e.target?.result as string);
        const board = data.board || data; // Support both formats
        
        // Validate board structure
        if (board && typeof board.width === 'number' && typeof board.height === 'number' && Array.isArray(board.aliveCells)) {
          game.setBoard(board as BoardDto);
        } else {
          alert('Invalid board file format');
        }
      } catch (error) {
        console.error('Failed to import board:', error);
        alert('Failed to import board file');
      }
    };
    reader.readAsText(file);
    
    // Clear the input
    event.target.value = '';
  }, [game]);

  const handleShare = useCallback(() => {
    if (!game.gameState.board) return;

    // Create a shareable URL with board data
    const boardData = encodeURIComponent(JSON.stringify(game.gameState.board));
    const shareUrl = `${window.location.origin}${window.location.pathname}?board=${boardData}`;
    
    if (navigator.clipboard && window.isSecureContext) {
      navigator.clipboard.writeText(shareUrl).then(() => {
        alert('Share URL copied to clipboard!');
      });
    } else {
      // Fallback for insecure contexts
      const textArea = document.createElement('textarea');
      textArea.value = shareUrl;
      document.body.appendChild(textArea);
      textArea.select();
      document.execCommand('copy');
      document.body.removeChild(textArea);
      alert('Share URL copied to clipboard!');
    }
  }, [game.gameState.board]);

  // Load board from URL on mount
  useEffect(() => {
    const urlParams = new URLSearchParams(window.location.search);
    const boardData = urlParams.get('board');
    if (boardData) {
      try {
        const board = JSON.parse(decodeURIComponent(boardData)) as BoardDto;
        game.setBoard(board);
        // Clean up URL
        window.history.replaceState({}, '', window.location.pathname);
      } catch (error) {
        console.warn('Failed to load board from URL:', error);
      }
    }
  }, []);

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between h-16">
            <div className="flex items-center gap-3">
              <div className="text-2xl">🧬</div>
              <div>
                <h1 className="text-xl font-bold text-gray-900">Conway's Game of Life</h1>
                <p className="text-sm text-gray-600">Interactive cellular automaton simulator</p>
              </div>
            </div>

            <div className="flex items-center gap-2">
              {/* API Health Indicator */}
              <div className="flex items-center gap-2 text-sm">
                <div className={`w-2 h-2 rounded-full ${isApiHealthy ? 'bg-green-500' : 'bg-red-500'}`} />
                <span className={isApiHealthy ? 'text-green-700' : 'text-red-700'}>
                  API {isApiHealthy ? 'Connected' : 'Disconnected'}
                </span>
              </div>

              {/* File Operations */}
              <label className="control-button control-button-secondary cursor-pointer">
                <Upload size={16} />
                Import
                <input
                  type="file"
                  accept=".json"
                  onChange={handleImport}
                  className="hidden"
                />
              </label>

              <button
                onClick={handleExport}
                disabled={!game.gameState.board}
                className="control-button control-button-secondary"
              >
                <Download size={16} />
                Export
              </button>

              <button
                onClick={handleShare}
                disabled={!game.gameState.board}
                className="control-button control-button-secondary"
              >
                <Share2 size={16} />
                Share
              </button>

              <button
                onClick={() => setShowPatternLibrary(!showPatternLibrary)}
                className={`control-button ${showPatternLibrary ? 'control-button-primary' : 'control-button-secondary'}`}
              >
                <Activity size={16} />
                Patterns
              </button>
            </div>
          </div>
        </div>
      </header>

      {/* API Error Banner */}
      {apiError && (
        <div className="bg-red-50 border-b border-red-200 px-4 py-2">
          <div className="max-w-7xl mx-auto flex items-center gap-2 text-red-700">
            <AlertCircle size={16} />
            <span className="text-sm">
              Unable to connect to API. Some features may be limited.
            </span>
          </div>
        </div>
      )}

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

            {/* Performance Metrics */}
            {game.performanceMetrics && (
              <div className="status-panel">
                <h3 className="font-semibold mb-2">Performance</h3>
                <div className="space-y-1 text-sm">
                  <div className="flex justify-between">
                    <span>Generation Time:</span>
                    <span className="metric-value">{game.performanceMetrics.generationTime.toFixed(1)}ms</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Speed:</span>
                    <span className="metric-value">{game.performanceMetrics.generationsPerSecond.toFixed(1)}/s</span>
                  </div>
                </div>
              </div>
            )}

            {/* Error Display */}
            {game.error && (
              <div className="bg-red-50 border border-red-200 rounded-lg p-3">
                <div className="flex items-start gap-2">
                  <AlertCircle size={16} className="text-red-500 mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-red-800">Error</p>
                    <p className="text-sm text-red-600">{game.error}</p>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>

        {/* Pattern Library Modal/Panel */}
        {showPatternLibrary && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
            <div className="bg-white rounded-lg shadow-xl max-w-4xl w-full max-h-[90vh] overflow-hidden">
              <div className="flex items-center justify-between p-4 border-b">
                <h2 className="text-lg font-semibold">Pattern Library</h2>
                <button
                  onClick={() => setShowPatternLibrary(false)}
                  className="text-gray-400 hover:text-gray-600"
                  aria-label="Close pattern library"
                >
                  ✕
                </button>
              </div>
              <div className="overflow-y-auto max-h-[calc(90vh-5rem)]">
                <PatternLibrary
                  onPatternSelect={handlePatternSelect}
                  onInsertPattern={handlePatternInsert}
                  currentBoard={game.gameState.board}
                />
              </div>
            </div>
          </div>
        )}

        {/* Selected Pattern Info */}
        {selectedPattern && (
          <div className="fixed bottom-4 right-4 bg-white rounded-lg shadow-lg border p-4 max-w-xs">
            <div className="flex items-start justify-between mb-2">
              <h3 className="font-semibold">{selectedPattern.name}</h3>
              <button
                onClick={() => setSelectedPattern(null)}
                className="text-gray-400 hover:text-gray-600 ml-2"
              >
                ✕
              </button>
            </div>
            <p className="text-sm text-gray-600 mb-2">{selectedPattern.description}</p>
            {selectedPattern.discoverer && (
              <p className="text-xs text-gray-500">By {selectedPattern.discoverer}</p>
            )}
          </div>
        )}
      </main>
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
