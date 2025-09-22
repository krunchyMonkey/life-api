import React from 'react';

interface SpeedControlProps {
  speed: number;
  isLoading: boolean;
  onSpeedChange: (speed: number) => void;
}

export const SpeedControl: React.FC<SpeedControlProps> = ({
  speed,
  isLoading,
  onSpeedChange
}) => {
  return (
    <div className="flex items-center gap-2">
      <label htmlFor="speed-slider" className="form-label text-sm mb-0">
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
        className="w-20 h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer slider:bg-blue-500"
        disabled={isLoading}
      />
      <span className="text-sm text-gray-600 w-12 font-mono">
        {speed.toFixed(1)}x
      </span>
    </div>
  );
};