import React from 'react';
import { Upload, Download, Share2, Activity } from 'lucide-react';

interface AppHeaderProps {
  isApiHealthy?: boolean;
  onImport: (event: React.ChangeEvent<HTMLInputElement>) => void;
  onExport: () => void;
  onShare: () => void;
  onTogglePatternLibrary: () => void;
  showPatternLibrary: boolean;
  hasBoard: boolean;
}

export const AppHeader: React.FC<AppHeaderProps> = ({
  isApiHealthy,
  onImport,
  onExport,
  onShare,
  onTogglePatternLibrary,
  showPatternLibrary,
  hasBoard
}) => {
  return (
    <header className="bg-white shadow-sm border-b">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          <div className="flex items-center gap-3">
            <div className="text-2xl">🧬</div>
            <div>
              <h1 className="text-xl font-bold text-gray-900">Conway's Game of Life</h1>
              <p className="text-sm text-gray-600">Interactive cellular automaton simulator</p>
            </div>
          </div>

          <div className="flex items-center gap-2">
            {/* API Health Indicator */}
            <div className="flex items-center gap-2 text-sm">
              <div className={`w-2 h-2 rounded-full ${isApiHealthy ? 'bg-green-500' : 'bg-red-500'}`} />
              <span className={isApiHealthy ? 'text-green-700' : 'text-red-700'}>
                API {isApiHealthy ? 'Connected' : 'Disconnected'}
              </span>
            </div>

            {/* File Operations */}
            <label className="control-button control-button-secondary cursor-pointer">
              <Upload size={16} />
              Import
              <input
                type="file"
                accept=".json"
                onChange={onImport}
                className="hidden"
              />
            </label>

            <button
              onClick={onExport}
              disabled={!hasBoard}
              className="control-button control-button-secondary"
            >
              <Download size={16} />
              Export
            </button>

            <button
              onClick={onShare}
              disabled={!hasBoard}
              className="control-button control-button-secondary"
            >
              <Share2 size={16} />
              Share
            </button>

            <button
              onClick={onTogglePatternLibrary}
              className={`control-button ${showPatternLibrary ? 'control-button-primary' : 'control-button-secondary'}`}
            >
              <Activity size={16} />
              Patterns
            </button>
          </div>
        </div>
      </div>
    </header>
  );
};