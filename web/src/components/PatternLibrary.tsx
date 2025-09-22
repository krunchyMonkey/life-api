import React, { useState, useMemo } from 'react';
import { Search, Plus, Eye, Info } from 'lucide-react';
import { CONWAY_PATTERNS, centerPattern, getPatternDimensions } from '../constants/patterns';
import type { ConwayPattern, BoardDto, Position } from '../types/api';

interface PatternLibraryProps {
  onPatternSelect: (pattern: ConwayPattern) => void;
  onInsertPattern: (positions: Position[]) => void;
  currentBoard: BoardDto | null;
  className?: string;
}

interface PatternPreviewProps {
  pattern: ConwayPattern;
  size?: number;
}

// Component to render a small preview of the pattern
const PatternPreview: React.FC<PatternPreviewProps> = ({ pattern, size = 60 }) => {
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

// Individual pattern card component
const PatternCard: React.FC<{
  pattern: ConwayPattern;
  onSelect: () => void;
  onInsert: () => void;
  canInsert: boolean;
}> = ({ pattern, onSelect, onInsert, canInsert }) => {
  const [showInfo, setShowInfo] = useState(false);
  const { width, height } = getPatternDimensions(pattern.positions);

  return (
    <div className="pattern-card">
      <div className="flex items-start justify-between mb-2">
        <h3 className="font-semibold text-sm truncate flex-1">{pattern.name}</h3>
        <button
          onClick={() => setShowInfo(!showInfo)}
          className="text-gray-400 hover:text-gray-600 ml-1"
          aria-label="Show pattern information"
        >
          <Info size={14} />
        </button>
      </div>

      <PatternPreview pattern={pattern} />

      <div className="mt-2">
        <p className="text-xs text-gray-600 mb-2 line-clamp-2">
          {pattern.description}
        </p>

        <div className="text-xs text-gray-500 space-y-1">
          <div>Size: {width}×{height}</div>
          <div>Cells: {pattern.positions.length}</div>
          {pattern.year && <div>Year: {pattern.year}</div>}
        </div>

        {showInfo && (
          <div className="mt-2 p-2 bg-gray-50 rounded text-xs">
            {pattern.discoverer && (
              <div><strong>Discoverer:</strong> {pattern.discoverer}</div>
            )}
            <div><strong>Description:</strong> {pattern.description}</div>
          </div>
        )}

        <div className="flex gap-2 mt-3">
          <button
            onClick={onSelect}
            className="btn flex-1 flex items-center justify-center gap-1 text-xs"
          >
            <Eye size={12} />
            View
          </button>
          <button
            onClick={onInsert}
            disabled={!canInsert}
            className={`flex-1 flex items-center justify-center gap-1 text-xs ${
              canInsert ? 'btn btn-success' : 'btn'
            }`}
            title={canInsert ? 'Insert pattern into board' : 'No board loaded'}
          >
            <Plus size={12} />
            Insert
          </button>
        </div>
      </div>
    </div>
  );
};

export const PatternLibrary: React.FC<PatternLibraryProps> = ({
  onPatternSelect,
  onInsertPattern,
  currentBoard,
  className = ''
}) => {
  const [selectedCategory, setSelectedCategory] = useState<keyof typeof CONWAY_PATTERNS>('stillLifes');
  const [searchQuery, setSearchQuery] = useState('');

  const categories = Object.keys(CONWAY_PATTERNS) as Array<keyof typeof CONWAY_PATTERNS>;

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

  const categoryLabels: Record<keyof typeof CONWAY_PATTERNS, string> = {
    stillLifes: 'Still Lifes',
    oscillators: 'Oscillators', 
    spaceships: 'Spaceships',
    guns: 'Guns',
    methuselahs: 'Methuselahs'
  };

  const categoryDescriptions: Record<keyof typeof CONWAY_PATTERNS, string> = {
    stillLifes: 'Patterns that remain unchanged',
    oscillators: 'Patterns that repeat after N generations',
    spaceships: 'Patterns that move across the board',
    guns: 'Patterns that create other patterns',
    methuselahs: 'Long-lived patterns that eventually stabilize'
  };

  return (
    <div className={`info-card ${className}`}>
      <div className="p-4 border-b border-gray-200">
        <h2 className="text-xl font-bold mb-4 text-gray-800">Pattern Library</h2>

        {/* Search */}
        <div className="relative mb-4">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={16} />
          <input
            type="text"
            placeholder="Search patterns..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="form-input w-full pl-10"
          />
        </div>

        {/* Category Tabs */}
        <div className="flex flex-wrap gap-2 mb-3">
          {categories.map(category => (
            <button
              key={category}
              onClick={() => setSelectedCategory(category)}
              className={`px-4 py-2 text-sm rounded-full transition-all duration-200 font-medium ${
                selectedCategory === category
                  ? 'btn-primary'
                  : 'btn'
              }`}
            >
              {categoryLabels[category]}
            </button>
          ))}
        </div>

        {/* Category Description */}
        <p className="text-sm text-gray-600 mt-2">
          {categoryDescriptions[selectedCategory]}
        </p>
      </div>

      <div className="p-4">
        {/* Results Count */}
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

        {/* Pattern Grid */}
        {filteredPatterns.length > 0 ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 max-h-96 overflow-y-auto">
            {filteredPatterns.map((pattern, index) => (
              <PatternCard
                key={`${pattern.name}-${index}`}
                pattern={pattern}
                onSelect={() => onPatternSelect(pattern)}
                onInsert={() => handlePatternInsert(pattern)}
                canInsert={currentBoard !== null}
              />
            ))}
          </div>
        ) : (
          <div className="text-center py-8">
            <div className="text-gray-400 text-4xl mb-2">🔍</div>
            <p className="text-gray-600">No patterns found</p>
            <p className="text-sm text-gray-500 mt-1">Try adjusting your search or selecting a different category</p>
          </div>
        )}
      </div>

      {/* Quick Stats */}
      <div className="border-t px-4 py-2 bg-gray-50">
        <div className="flex justify-between items-center text-xs text-gray-600">
          <span>
            Total patterns: {Object.values(CONWAY_PATTERNS).flat().length}
          </span>
          <span className="flex items-center gap-1">
            <span className="w-2 h-2 bg-green-500 rounded-full"></span>
            Alive Cell
            <span className="w-2 h-2 bg-gray-200 rounded-full ml-2"></span>
            Dead Cell
          </span>
        </div>
      </div>
    </div>
  );
};

export default PatternLibrary;