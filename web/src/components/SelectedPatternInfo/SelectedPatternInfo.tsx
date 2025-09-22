import React from 'react';
import type { ConwayPattern } from '../../types/api';

interface SelectedPatternInfoProps {
  pattern: ConwayPattern | null;
  onClose: () => void;
}

export const SelectedPatternInfo: React.FC<SelectedPatternInfoProps> = ({ pattern, onClose }) => {
  if (!pattern) return null;

  return (
    <div className="fixed bottom-4 right-4 bg-white rounded-lg shadow-lg border p-4 max-w-xs">
      <div className="flex items-start justify-between mb-2">
        <h3 className="font-semibold">{pattern.name}</h3>
        <button
          onClick={onClose}
          className="text-gray-400 hover:text-gray-600 ml-2"
        >
          ✕
        </button>
      </div>
      <p className="text-sm text-gray-600 mb-2">{pattern.description}</p>
      {pattern.discoverer && (
        <p className="text-xs text-gray-500">By {pattern.discoverer}</p>
      )}
    </div>
  );
};