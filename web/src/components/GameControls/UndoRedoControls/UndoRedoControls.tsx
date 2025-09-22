import React from 'react';
import { Undo2, Redo2 } from 'lucide-react';
import type { GameState } from '../../../types/api';

interface UndoRedoControlsProps {
  gameState: GameState;
  isLoading: boolean;
  canUndo: boolean;
  canRedo: boolean;
  onUndo: () => void;
  onRedo: () => void;
}

export const UndoRedoControls: React.FC<UndoRedoControlsProps> = ({
  gameState,
  isLoading,
  canUndo,
  canRedo,
  onUndo,
  onRedo
}) => {
  return (
    <div className="flex items-center gap-2 flex-wrap">
      <button
        onClick={onUndo}
        disabled={!canUndo || isLoading}
        className="btn flex items-center gap-2"
        aria-label="Undo last action"
      >
        <Undo2 size={16} />
        Undo
      </button>

      <button
        onClick={onRedo}
        disabled={!canRedo || isLoading}
        className="btn flex items-center gap-2"
        aria-label="Redo last action"
      >
        <Redo2 size={16} />
        Redo
      </button>

      {(canUndo || canRedo) && (
        <span className="text-sm text-gray-500">
          {gameState.history.length} states in history
        </span>
      )}
    </div>
  );
};