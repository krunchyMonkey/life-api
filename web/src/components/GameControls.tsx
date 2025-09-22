import React, { useState } from 'react';
import { 
  Play, 
  Pause, 
  SkipForward, 
  FastForward, 
  Square, 
  RefreshCw, 
  Shuffle, 
  Undo2, 
  Redo2,
  Zap,
  Settings
} from 'lucide-react';
import type { GameState } from '../types/api';

interface GameControlsProps {
  gameState: GameState;
  isLoading: boolean;
  onPlay: () => void;
  onPause: () => void;
  onNextGeneration: () => void;
  onAdvanceGenerations: (n: number) => void;
  onGetFinalState: () => void;
  onReset: () => void;
  onClear: () => void;
  onRandomize: (density?: number) => void;
  onSpeedChange: (speed: number) => void;
  onUndo: () => void;
  onRedo: () => void;
  canUndo: boolean;
  canRedo: boolean;
  className?: string;
}

export const GameControls: React.FC<GameControlsProps> = ({
  gameState,
  isLoading,
  onPlay,
  onPause,
  onNextGeneration,
  onAdvanceGenerations,
  onGetFinalState,
  onReset,
  onClear,
  onRandomize,
  onSpeedChange,
  onUndo,
  onRedo,
  canUndo,
  canRedo,
  className = ''
}) => {
  const [advanceCount, setAdvanceCount] = useState(10);
  const [randomDensity, setRandomDensity] = useState(0.3);
  const [showAdvanced, setShowAdvanced] = useState(false);

  const { board, isPlaying, speed } = gameState;
  const hasBoard = board !== null;
  const aliveCellsCount = board?.aliveCells.length || 0;

  const handleAdvanceSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (advanceCount > 0) {
      onAdvanceGenerations(advanceCount);
    }
  };

  const handleRandomizeClick = () => {
    onRandomize(randomDensity);
  };

  return (
    <div className={`bg-white rounded-lg shadow-lg border p-4 space-y-4 ${className}`}>
      {/* Primary Controls */}
      <div className="flex items-center gap-2">
        {/* Play/Pause */}
        <button
          onClick={isPlaying ? onPause : onPlay}
          disabled={!hasBoard || isLoading}
          className={`control-button ${
            isPlaying ? 'control-button-danger' : 'control-button-primary'
          } flex items-center gap-2`}
          aria-label={isPlaying ? 'Pause simulation' : 'Play simulation'}
        >
          {isPlaying ? <Pause size={16} /> : <Play size={16} />}
          {isPlaying ? 'Pause' : 'Play'}
        </button>

        {/* Step */}
        <button
          onClick={onNextGeneration}
          disabled={!hasBoard || isLoading || isPlaying}
          className="control-button control-button-secondary flex items-center gap-2"
          aria-label="Advance one generation"
        >
          <SkipForward size={16} />
          Step
        </button>

        {/* Speed Control */}
        <div className="flex items-center gap-2">
          <label htmlFor="speed-slider" className="text-sm font-medium text-gray-700">
            Speed:
          </label>
          <input
            id="speed-slider"
            type="range"
            min="0.1"
            max="20"
            step="0.1"
            value={speed}
            onChange={(e) => onSpeedChange(parseFloat(e.target.value))}
            className="w-20 h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer"
            disabled={isLoading}
          />
          <span className="text-sm text-gray-600 w-12">
            {speed.toFixed(1)}x
          </span>
        </div>

        {/* Advanced Controls Toggle */}
        <button
          onClick={() => setShowAdvanced(!showAdvanced)}
          className="control-button control-button-secondary ml-auto"
          aria-label="Toggle advanced controls"
        >
          <Settings size={16} />
        </button>
      </div>

      {/* Board Info */}
      {hasBoard && (
        <div className="flex items-center gap-4 text-sm text-gray-600 bg-gray-50 p-2 rounded">
          <span>
            <strong>Generation:</strong> {board.generation}
          </span>
          <span>
            <strong>Size:</strong> {board.width}×{board.height}
          </span>
          <span>
            <strong>Alive Cells:</strong> {aliveCellsCount}
          </span>
          <span>
            <strong>Density:</strong> {((aliveCellsCount / (board.width * board.height)) * 100).toFixed(1)}%
          </span>
        </div>
      )}

      {/* Advanced Controls */}
      {showAdvanced && (
        <div className="space-y-3 border-t pt-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Advance N Generations */}
            <form onSubmit={handleAdvanceSubmit} className="flex items-center gap-2">
              <FastForward size={16} className="text-gray-500" />
              <input
                type="number"
                min="1"
                max="1000"
                value={advanceCount}
                onChange={(e) => setAdvanceCount(parseInt(e.target.value) || 1)}
                className="w-16 px-2 py-1 text-sm border rounded focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                disabled={isLoading}
              />
              <button
                type="submit"
                disabled={!hasBoard || isLoading || isPlaying}
                className="control-button control-button-secondary text-sm"
              >
                Advance
              </button>
            </form>

            {/* Get Final State */}
            <button
              onClick={onGetFinalState}
              disabled={!hasBoard || isLoading || isPlaying}
              className="control-button control-button-secondary flex items-center gap-2 justify-center"
            >
              <Zap size={16} />
              Final State
            </button>
          </div>

          {/* Board Management */}
          <div className="flex flex-wrap items-center gap-2">
            <div className="flex items-center gap-2">
              <Shuffle size={16} className="text-gray-500" />
              <input
                type="number"
                min="0.1"
                max="0.9"
                step="0.1"
                value={randomDensity}
                onChange={(e) => setRandomDensity(parseFloat(e.target.value))}
                className="w-16 px-2 py-1 text-sm border rounded focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                disabled={isLoading}
              />
              <button
                onClick={handleRandomizeClick}
                disabled={!hasBoard || isLoading}
                className="control-button control-button-secondary text-sm"
              >
                Randomize
              </button>
            </div>

            <button
              onClick={onClear}
              disabled={!hasBoard || isLoading}
              className="control-button control-button-secondary flex items-center gap-2"
            >
              <Square size={16} />
              Clear
            </button>

            <button
              onClick={onReset}
              disabled={!hasBoard || isLoading || gameState.history.length === 0}
              className="control-button control-button-secondary flex items-center gap-2"
            >
              <RefreshCw size={16} />
              Reset
            </button>
          </div>

          {/* Undo/Redo */}
          <div className="flex items-center gap-2">
            <button
              onClick={onUndo}
              disabled={!canUndo || isLoading}
              className="control-button control-button-secondary flex items-center gap-2"
              aria-label="Undo last action"
            >
              <Undo2 size={16} />
              Undo
            </button>

            <button
              onClick={onRedo}
              disabled={!canRedo || isLoading}
              className="control-button control-button-secondary flex items-center gap-2"
              aria-label="Redo last action"
            >
              <Redo2 size={16} />
              Redo
            </button>

            {(canUndo || canRedo) && (
              <span className="text-sm text-gray-500">
                {gameState.history.length} states in history
              </span>
            )}
          </div>
        </div>
      )}

      {/* Loading Indicator */}
      {isLoading && (
        <div className="flex items-center justify-center gap-2 py-2">
          <div className="animate-spin rounded-full h-4 w-4 border-2 border-blue-500 border-t-transparent"></div>
          <span className="text-sm text-gray-600">Computing...</span>
        </div>
      )}

      {/* Keyboard Shortcuts Help */}
      {showAdvanced && (
        <div className="text-xs text-gray-500 bg-gray-50 p-2 rounded">
          <strong>Keyboard Shortcuts:</strong> 
          <div className="grid grid-cols-2 gap-1 mt-1">
            <span>Space: Play/Pause</span>
            <span>→: Next Generation</span>
            <span>Ctrl+Z: Undo</span>
            <span>Ctrl+Y: Redo</span>
            <span>R: Randomize</span>
            <span>C: Clear</span>
          </div>
        </div>
      )}
    </div>
  );
};

export default GameControls;