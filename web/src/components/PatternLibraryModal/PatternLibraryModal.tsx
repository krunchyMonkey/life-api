import React from 'react';
import { PatternLibrary } from '../PatternLibrary';
import type { ConwayPattern, BoardDto, Position } from '../../types/api';

interface PatternLibraryModalProps {
  isOpen: boolean;
  onClose: () => void;
  onPatternSelect: (pattern: ConwayPattern) => void;
  onInsertPattern: (positions: Position[]) => void;
  currentBoard: BoardDto | null;
}

export const PatternLibraryModal: React.FC<PatternLibraryModalProps> = ({
  isOpen,
  onClose,
  onPatternSelect,
  onInsertPattern,
  currentBoard
}) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg shadow-xl max-w-4xl w-full max-h-[90vh] overflow-hidden">
        <div className="flex items-center justify-between p-4 border-b">
          <h2 className="text-lg font-semibold">Pattern Library</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600"
            aria-label="Close pattern library"
          >
            ✕
          </button>
        </div>
        <div className="overflow-y-auto max-h-[calc(90vh-5rem)]">
          <PatternLibrary
            onPatternSelect={onPatternSelect}
            onInsertPattern={onInsertPattern}
            currentBoard={currentBoard}
          />
        </div>
      </div>
    </div>
  );
};