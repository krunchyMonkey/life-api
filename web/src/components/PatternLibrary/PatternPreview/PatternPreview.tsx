import React from 'react';
import { getPatternDimensions } from '../../../constants/patterns';
import type { ConwayPattern } from '../../../types/api';

interface PatternPreviewProps {
  pattern: ConwayPattern;
  size?: number;
}

export const PatternPreview: React.FC<PatternPreviewProps> = ({ pattern, size = 60 }) => {
  const { width, height } = getPatternDimensions(pattern.positions);
  const maxDim = Math.max(width, height);
  const cellSize = Math.max(1, Math.floor(size / maxDim));
  const actualSize = maxDim * cellSize;

  // Create a set for fast lookup
  const cellSet = new Set(pattern.positions.map(pos => `${pos.x},${pos.y}`));

  // Find bounds for centering
  const minX = Math.min(...pattern.positions.map(p => p.x));
  const minY = Math.min(...pattern.positions.map(p => p.y));

  return (
    <div 
      className="pattern-preview mx-auto"
      style={{
        width: actualSize,
        height: actualSize,
        display: 'grid',
        gridTemplateColumns: `repeat(${width}, ${cellSize}px)`,
        gridTemplateRows: `repeat(${height}, ${cellSize}px)`,
        gap: '0.5px'
      }}
    >
      {Array.from({ length: height }, (_, y) =>
        Array.from({ length: width }, (_, x) => {
          const isAlive = cellSet.has(`${x + minX},${y + minY}`);
          return (
            <div
              key={`${x}-${y}`}
              className={`pattern-cell ${isAlive ? 'alive' : ''}`}
              style={{ width: cellSize, height: cellSize }}
            />
          );
        })
      )}
    </div>
  );
};