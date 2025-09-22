import React from 'react';

interface KeyboardShortcutsProps {
  showAdvanced: boolean;
}

export const KeyboardShortcuts: React.FC<KeyboardShortcutsProps> = ({ showAdvanced }) => {
  if (!showAdvanced) return null;

  return (
    <div className="text-xs text-gray-500 bg-gray-50 p-3 rounded-lg border border-gray-100">
      <div className="font-semibold text-gray-700 mb-2">Keyboard Shortcuts:</div> 
      <div className="grid grid-cols-2 gap-x-4 gap-y-1">
        <span><kbd className="px-1 py-0.5 bg-white border rounded text-xs">Space</kbd> Play/Pause</span>
        <span><kbd className="px-1 py-0.5 bg-white border rounded text-xs">→</kbd> Next Generation</span>
        <span><kbd className="px-1 py-0.5 bg-white border rounded text-xs">Ctrl+Z</kbd> Undo</span>
        <span><kbd className="px-1 py-0.5 bg-white border rounded text-xs">Ctrl+Y</kbd> Redo</span>
        <span><kbd className="px-1 py-0.5 bg-white border rounded text-xs">R</kbd> Randomize</span>
        <span><kbd className="px-1 py-0.5 bg-white border rounded text-xs">C</kbd> Clear</span>
      </div>
    </div>
  );
};