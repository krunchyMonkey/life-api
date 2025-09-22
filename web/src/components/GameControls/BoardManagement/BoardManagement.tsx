import React, { useState } from 'react';
import { Shuffle, Square, RefreshCw } from 'lucide-react';
import type { GameState } from '../../../types/api';

interface BoardManagementProps {
  hasBoard: boolean;
  isLoading: boolean;
  gameState: GameState;
  onRandomize: (density?: number) => void;
  onClear: () => void;
  onReset: () => void;
}

export const BoardManagement: React.FC<BoardManagementProps> = ({
  hasBoard,
  isLoading,
  gameState,
  onRandomize,
  onClear,
  onReset
}) => {
  const [randomDensity, setRandomDensity] = useState(0.3);

  const handleRandomizeClick = () => {
    onRandomize(randomDensity);
  };

  return (
    <div className="flex flex-wrap items-center gap-3">
      <div className="flex items-center gap-2">
        <Shuffle size={16} className="text-gray-500" />
        <input
          type="number"
          min="0.1"
          max="0.9"
          step="0.1"
          value={randomDensity}
          onChange={(e) => setRandomDensity(parseFloat(e.target.value))}
          className="form-input w-16 text-sm"
          disabled={isLoading}
        />
        <button
          onClick={handleRandomizeClick}
          disabled={!hasBoard || isLoading}
          className="btn btn-success text-sm"
        >
          Randomize
        </button>
      </div>

      <button
        onClick={onClear}
        disabled={!hasBoard || isLoading}
        className="btn flex items-center gap-2"
      >
        <Square size={16} />
        Clear
      </button>

      <button
        onClick={onReset}
        disabled={!hasBoard || isLoading || gameState.history.length === 0}
        className="btn flex items-center gap-2"
      >
        <RefreshCw size={16} />
        Reset
      </button>
    </div>
  );
};