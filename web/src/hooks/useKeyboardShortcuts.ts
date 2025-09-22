import { useEffect } from 'react';

interface KeyboardShortcutsProps {
  isPlaying: boolean;
  isLoading: boolean;
  onPlayPause: () => void;
  onNextGeneration: () => void;
  onRandomize: () => void;
  onClear: () => void;
  onUndo: () => void;
  onRedo: () => void;
}

export const useKeyboardShortcuts = ({
  isPlaying,
  isLoading,
  onPlayPause,
  onNextGeneration,
  onRandomize,
  onClear,
  onUndo,
  onRedo
}: KeyboardShortcutsProps) => {
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      // Only handle shortcuts when not typing in an input
      if (e.target instanceof HTMLInputElement || e.target instanceof HTMLTextAreaElement) {
        return;
      }

      switch (e.key) {
        case ' ':
          e.preventDefault();
          onPlayPause();
          break;
        case 'ArrowRight':
          if (!isPlaying && !isLoading) {
            e.preventDefault();
            onNextGeneration();
          }
          break;
        case 'r':
        case 'R':
          if (!isLoading) {
            e.preventDefault();
            onRandomize();
          }
          break;
        case 'c':
        case 'C':
          if (!isLoading) {
            e.preventDefault();
            onClear();
          }
          break;
        case 'z':
          if (e.ctrlKey && !isLoading) {
            e.preventDefault();
            onUndo();
          }
          break;
        case 'y':
          if (e.ctrlKey && !isLoading) {
            e.preventDefault();
            onRedo();
          }
          break;
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [isPlaying, isLoading, onPlayPause, onNextGeneration, onRandomize, onClear, onUndo, onRedo]);
};