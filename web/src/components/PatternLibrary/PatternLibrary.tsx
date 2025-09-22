import React, { useState, useMemo } from 'react';
import { CONWAY_PATTERNS, centerPattern } from '../../constants/patterns';
import type { ConwayPattern, BoardDto, Position } from '../../types/api';
import { SearchBar } from './SearchBar';
import { CategoryTabs } from './CategoryTabs';
import { PatternGrid } from './PatternGrid';
import { ResultsCount, LibraryFooter } from './LibraryStats';
import type { CategoryType } from './constants';
import { categoryLabels, categoryDescriptions } from './constants';

interface PatternLibraryProps {
  onPatternSelect: (pattern: ConwayPattern) => void;
  onInsertPattern: (positions: Position[]) => void;
  currentBoard: BoardDto | null;
  className?: string;
}

export const PatternLibrary: React.FC<PatternLibraryProps> = ({
  onPatternSelect,
  onInsertPattern,
  currentBoard,
  className = ''
}) => {
  const [selectedCategory, setSelectedCategory] = useState<CategoryType>('stillLifes');
  const [searchQuery, setSearchQuery] = useState('');

  // Filter patterns based on search query
  const filteredPatterns = useMemo(() => {
    const patterns = CONWAY_PATTERNS[selectedCategory] || [];
    if (!searchQuery) return patterns;

    const query = searchQuery.toLowerCase();
    return patterns.filter(pattern => 
      pattern.name.toLowerCase().includes(query) ||
      pattern.description.toLowerCase().includes(query) ||
      pattern.discoverer?.toLowerCase().includes(query)
    );
  }, [selectedCategory, searchQuery]);

  const handlePatternInsert = (pattern: ConwayPattern) => {
    if (!currentBoard) return;

    // Center the pattern on the board
    const centeredPositions = centerPattern(
      pattern.positions, 
      currentBoard.width, 
      currentBoard.height
    );

    onInsertPattern(centeredPositions);
  };

  const totalPatternCount = Object.values(CONWAY_PATTERNS).flat().length;

  return (
    <div className={`info-card ${className}`}>
      <div className="p-4 border-b border-gray-200">
        <h2 className="text-xl font-bold mb-4 text-gray-800">Pattern Library</h2>

        <SearchBar 
          searchQuery={searchQuery}
          onSearchChange={setSearchQuery}
        />

        <CategoryTabs
          selectedCategory={selectedCategory}
          onCategoryChange={setSelectedCategory}
          categoryLabels={categoryLabels}
          categoryDescriptions={categoryDescriptions}
        />
      </div>

      <div className="p-4">
        <ResultsCount
          filteredPatterns={filteredPatterns}
          currentBoard={currentBoard}
        />

        <PatternGrid
          patterns={filteredPatterns}
          currentBoard={currentBoard}
          onPatternSelect={onPatternSelect}
          onPatternInsert={handlePatternInsert}
        />
      </div>

      <LibraryFooter totalPatternCount={totalPatternCount} />
    </div>
  );
};

export default PatternLibrary;