import React from 'react';
import { Play, Pause, SkipForward } from 'lucide-react';

interface PrimaryControlsProps {
  isPlaying: boolean;
  hasBoard: boolean;
  isLoading: boolean;
  onPlay: () => void;
  onPause: () => void;
  onNextGeneration: () => void;
}

export const PrimaryControls: React.FC<PrimaryControlsProps> = ({
  isPlaying,
  hasBoard,
  isLoading,
  onPlay,
  onPause,
  onNextGeneration
}) => {
  return (
    <div className="flex items-center gap-3">
      {/* Play/Pause */}
      <button
        onClick={isPlaying ? onPause : onPlay}
        disabled={!hasBoard || isLoading}
        className={`btn ${
          isPlaying ? 'btn-danger' : 'btn-primary'
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
        className="btn flex items-center gap-2"
        aria-label="Advance one generation"
      >
        <SkipForward size={16} />
        Step
      </button>
    </div>
  );
};