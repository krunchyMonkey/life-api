import { useRef, useEffect } from 'react';
import type { BoardDto } from '../../types/api';
import Cell from './Cell';
import { useAliveCells, useBoardDimensions, BoardUtils } from './GameBoard.utils';
import { useGameBoardViewport } from '../../hooks/useGameBoardViewport';
import { 
  BoardInfoOverlay, 
  BoardControlsOverlay, 
  PerformanceHintOverlay, 
  ReadOnlyOverlay 
} from './BoardOverlays';
import  EmptyBoard  from './EmptyBoard';

interface GameBoardProps {
  board: BoardDto | null;
  onCellClick?: (x: number, y: number) => void;
  cellSize?: number;
  showGrid?: boolean;
  className?: string;
  animateChanges?: boolean;
  readOnly?: boolean;
}

export const GameBoard: React.FC<GameBoardProps> = ({
  board,
  onCellClick,
  cellSize = 12,
  showGrid = true,
  className = '',
  animateChanges = true,
  readOnly = false
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const { viewport, handlers, resetViewport } = useGameBoardViewport();
  const { zoom, pan, isDragging } = viewport;
  
  // Create lookup set for alive cells
  const aliveCellsSet = useAliveCells(board);
  
  // Calculate board dimensions
  const boardDimensions = useBoardDimensions(board, cellSize, zoom);

  // Reset viewport when board changes
  useEffect(() => {
    resetViewport();
  }, [board?.width, board?.height, resetViewport]);

  // Handle keyboard shortcuts
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.target !== document.body) return;

      switch (e.key) {
        case '+':
        case '=':
          e.preventDefault();
          handlers.handleZoom(1.2);
          break;
        case '-':
          e.preventDefault();
          handlers.handleZoom(0.8);
          break;
        case '0':
          e.preventDefault();
          resetViewport();
          break;
        case 'ArrowUp':
          e.preventDefault();
          handlers.handlePan({ x: 0, y: 20 });
          break;
        case 'ArrowDown':
          e.preventDefault();
          handlers.handlePan({ x: 0, y: -20 });
          break;
        case 'ArrowLeft':
          e.preventDefault();
          handlers.handlePan({ x: 20, y: 0 });
          break;
        case 'ArrowRight':
          e.preventDefault();
          handlers.handlePan({ x: -20, y: 0 });
          break;
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [handlers, resetViewport]);

  if (!board) {
    return <EmptyBoard className={className} />;
  }

  return (
    <div 
      className={`game-board relative overflow-hidden ${className}`}
      ref={containerRef}
      onWheel={handlers.handleWheel}
      onMouseDown={handlers.handleMouseDown}
      onMouseMove={handlers.handleMouseMove}
      onMouseUp={handlers.handleMouseUp}
      onMouseLeave={handlers.handleMouseUp}
      style={{ 
        cursor: isDragging ? 'grabbing' : 'grab',
        userSelect: 'none'
      }}
      tabIndex={0}
      role="grid"
      aria-label={`Game of Life board, ${board.width} by ${board.height}, generation ${board.generation}`}
    >
      <BoardInfoOverlay board={board} zoom={zoom} />
      <BoardControlsOverlay />

      {/* Game Board Grid */}
      <div 
        className="absolute"
        style={{
          transform: `translate(${pan.x}px, ${pan.y}px) scale(${zoom})`,
          transformOrigin: '0 0',
          width: boardDimensions ? boardDimensions.width / zoom : 0,
          height: boardDimensions ? boardDimensions.height / zoom : 0,
          display: 'grid',
          ...BoardUtils.getGridTemplate(board, cellSize),
        }}
      >
        {Array.from({ length: board.height }, (_, y) =>
          Array.from({ length: board.width }, (_, x) => (
            <Cell
              key={`${x}-${y}`}
              isAlive={aliveCellsSet.has(`${x},${y}`)}
              x={x}
              y={y}
              size={cellSize}
              onClick={onCellClick}
              showGrid={showGrid}
              animated={animateChanges}
              readOnly={readOnly}
            />
          ))
        )}
      </div>

      <PerformanceHintOverlay board={board} />
      <ReadOnlyOverlay readOnly={readOnly} />
    </div>
  );
};

export default GameBoard;