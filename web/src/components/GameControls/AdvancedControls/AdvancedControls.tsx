import React, { useState } from 'react';
import { FastForward, Zap } from 'lucide-react';

interface AdvancedControlsProps {
  hasBoard: boolean;
  isLoading: boolean;
  isPlaying: boolean;
  onAdvanceGenerations: (n: number) => void;
  onGetFinalState: () => void;
}

export const AdvancedControls: React.FC<AdvancedControlsProps> = ({
  hasBoard,
  isLoading,
  isPlaying,
  onAdvanceGenerations,
  onGetFinalState
}) => {
  const [advanceCount, setAdvanceCount] = useState(10);

  const handleAdvanceSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (advanceCount > 0) {
      onAdvanceGenerations(advanceCount);
    }
  };

  return (
    <div className="flex flex-wrap items-center gap-3">
      {/* Advance N Generations */}
      <div className="flex items-center gap-2">
        <FastForward size={16} className="text-gray-500" />
        <form onSubmit={handleAdvanceSubmit} className="flex items-center gap-2">
          <input
            type="number"
            min="1"
            max="1000"
            value={advanceCount}
            onChange={(e) => setAdvanceCount(parseInt(e.target.value) || 1)}
            className="form-input w-16 text-sm"
            disabled={isLoading}
          />
          <button
            type="submit"
            disabled={!hasBoard || isLoading || isPlaying}
            className="btn text-sm"
          >
            Advance
          </button>
        </form>
      </div>

      {/* Get Final State */}
      <button
        onClick={onGetFinalState}
        disabled={!hasBoard || isLoading || isPlaying}
        className="btn btn-warning flex items-center gap-2"
      >
        <Zap size={16} />
        Final State
      </button>
    </div>
  );
};