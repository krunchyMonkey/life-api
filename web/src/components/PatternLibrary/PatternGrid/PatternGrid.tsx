import React from 'react';
import type { ConwayPattern, BoardDto } from '../../../types/api';
import { PatternCard } from '../PatternCard';

interface PatternGridProps {
  patterns: ConwayPattern[];
  currentBoard: BoardDto | null;
  onPatternSelect: (pattern: ConwayPattern) => void;
  onPatternInsert: (pattern: ConwayPattern) => void;
}

export const PatternGrid: React.FC<PatternGridProps> = ({
  patterns,
  currentBoard,
  onPatternSelect,
  onPatternInsert
}) => {
  if (patterns.length === 0) {
    return (
      <div className="text-center py-8">
        <div className="text-gray-400 text-4xl mb-2">🔍</div>
        <p className="text-gray-600">No patterns found</p>
        <p className="text-sm text-gray-500 mt-1">Try adjusting your search or selecting a different category</p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 max-h-96 overflow-y-auto">
      {patterns.map((pattern, index) => (
        <PatternCard
          key={`${pattern.name}-${index}`}
          pattern={pattern}
          onSelect={() => onPatternSelect(pattern)}
          onInsert={() => onPatternInsert(pattern)}
          canInsert={currentBoard !== null}
        />
      ))}
    </div>
  );
};