import React, { useState, memo } from 'react';
import { Settings } from 'lucide-react';
import type { GameState } from '../../types/api';
import { PrimaryControls } from './PrimaryControls';
import { SpeedControl } from './SpeedControl';
import { BoardStats } from './BoardStats';
import { AdvancedControls } from './AdvancedControls';
import { BoardManagement } from './BoardManagement';
import { UndoRedoControls } from './UndoRedoControls';
import { SmoothLoadingIndicator } from './LoadingIndicator';
import { KeyboardShortcuts } from './KeyboardShortcuts';

interface GameControlsProps {
  gameState: GameState;
  isLoading: boolean;
  onPlay: () => void;
  onPause: () => void;
  onNextGeneration: () => void;
  onAdvanceGenerations: (n: number) => void;
  onGetFinalState: () => void;
  onReset: () => void;
  onClear: () => void;
  onRandomize: (density?: number) => void;
  onSpeedChange: (speed: number) => void;
  onUndo: () => void;
  onRedo: () => void;
  canUndo: boolean;
  canRedo: boolean;
  className?: string;
}

const GameControls: React.FC<GameControlsProps> = ({
  gameState,
  isLoading,
  onPlay,
  onPause,
  onNextGeneration,
  onAdvanceGenerations,
  onGetFinalState,
  onReset,
  onClear,
  onRandomize,
  onSpeedChange,
  onUndo,
  onRedo,
  canUndo,
  canRedo,
  className = ''
}) => {
  const [showAdvanced, setShowAdvanced] = useState(false);

  const { board, isPlaying, speed } = gameState;
  const hasBoard = board !== null;

  return (
    <div className={`info-card space-y-4 ${className}`}>
      {/* Primary Controls */}
      <div className="flex items-center gap-3 flex-wrap">
        <PrimaryControls
          isPlaying={isPlaying}
          hasBoard={hasBoard}
          isLoading={isLoading}
          onPlay={onPlay}
          onPause={onPause}
          onNextGeneration={onNextGeneration}
        />

        <SpeedControl
          speed={speed}
          isLoading={isLoading}
          onSpeedChange={onSpeedChange}
        />

        {/* Advanced Controls Toggle */}
        <button
          onClick={() => setShowAdvanced(!showAdvanced)}
          className="btn ml-auto"
          aria-label="Toggle advanced controls"
        >
          <Settings size={16} />
        </button>
      </div>

      {/* Board Info */}
      {hasBoard && <BoardStats board={board} />}

      {/* Advanced Controls */}
      {showAdvanced && (
        <div className="space-y-4 border-t border-gray-200 pt-4">
          <AdvancedControls
            hasBoard={hasBoard}
            isLoading={isLoading}
            isPlaying={isPlaying}
            onAdvanceGenerations={onAdvanceGenerations}
            onGetFinalState={onGetFinalState}
          />

          <BoardManagement
            hasBoard={hasBoard}
            isLoading={isLoading}
            gameState={gameState}
            onRandomize={onRandomize}
            onClear={onClear}
            onReset={onReset}
          />

          <UndoRedoControls
            gameState={gameState}
            isLoading={isLoading}
            canUndo={canUndo}
            canRedo={canRedo}
            onUndo={onUndo}
            onRedo={onRedo}
          />
        </div>
      )}

      <SmoothLoadingIndicator 
        isLoading={isLoading} 
        loadingText="Computing..."
        showDelayMs={200}
        minDisplayMs={600}
      />
      
      <KeyboardShortcuts showAdvanced={showAdvanced} />
    </div>
  );
};

// Memoize to prevent unnecessary re-renders during gameplay
const MemoizedGameControls = memo(GameControls);
export { MemoizedGameControls as GameControls };
export default MemoizedGameControls;