import React, { useState } from 'react';
import { Plus, Eye, Info } from 'lucide-react';
import { getPatternDimensions } from '../../../constants/patterns';
import type { ConwayPattern } from '../../../types/api';
import { PatternPreview } from '../PatternPreview';

interface PatternCardProps {
  pattern: ConwayPattern;
  onSelect: () => void;
  onInsert: () => void;
  canInsert: boolean;
}

export const PatternCard: React.FC<PatternCardProps> = ({ 
  pattern, 
  onSelect, 
  onInsert, 
  canInsert 
}) => {
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