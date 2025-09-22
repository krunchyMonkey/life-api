import React from 'react';
import type { ConwayPattern, BoardDto } from '../../../types/api';

interface ResultsCountProps {
  filteredPatterns: ConwayPattern[];
  currentBoard: BoardDto | null;
}

interface LibraryFooterProps {
  totalPatternCount: number;
}

export const ResultsCount: React.FC<ResultsCountProps> = ({
  filteredPatterns,
  currentBoard
}) => {
  return (
    <div className="flex items-center justify-between mb-4">
      <span className="text-sm text-gray-600">
        {filteredPatterns.length} pattern{filteredPatterns.length !== 1 ? 's' : ''} found
      </span>
      {!currentBoard && (
        <span className="text-xs text-amber-600 bg-amber-50 px-2 py-1 rounded">
          Load a board to insert patterns
        </span>
      )}
    </div>
  );
};

export const LibraryFooter: React.FC<LibraryFooterProps> = ({ totalPatternCount }) => {
  return (
    <div className="border-t px-4 py-2 bg-gray-50">
      <div className="flex justify-between items-center text-xs text-gray-600">
        <span>
          Total patterns: {totalPatternCount}
        </span>
        <span className="flex items-center gap-1">
          <span className="w-2 h-2 bg-green-500 rounded-full"></span>
          Alive Cell
          <span className="w-2 h-2 bg-gray-200 rounded-full ml-2"></span>
          Dead Cell
        </span>
      </div>
    </div>
  );
};