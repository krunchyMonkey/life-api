import { useRef, useMemo, memo } from 'react';
import type { BoardDto } from '../../types/api';
import Cell from './Cell';
import { 
  useAliveCells, 
  useBoardDimensions, 
  useGameBoardKeyboardHandler,
  BoardUtils 
} from './GameBoard.utils';
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

// Custom comparison function to prevent unnecessary re-renders
const areEqual = (prevProps: GameBoardProps, nextProps: GameBoardProps): boolean => {
  // Quick reference check
  if (prevProps.board === nextProps.board) return true;
  
  // If one is null and the other isn't
  if (!prevProps.board || !nextProps.board) return false;
  
  // Compare board properties that affect rendering
  if (
    prevProps.board.width !== nextProps.board.width ||
    prevProps.board.height !== nextProps.board.height ||
    prevProps.board.generation !== nextProps.board.generation ||
    prevProps.board.aliveCells.length !== nextProps.board.aliveCells.length
  ) return false;
  
  // Compare alive cells efficiently
  if (prevProps.board.aliveCells.length > 0) {
    const prevCells = new Set(prevProps.board.aliveCells.map(c => `${c.x},${c.y}`));
    const nextCells = new Set(nextProps.board.aliveCells.map(c => `${c.x},${c.y}`));
    
    if (prevCells.size !== nextCells.size) return false;
    for (const cell of prevCells) {
      if (!nextCells.has(cell)) return false;
    }
  }
  
  // Compare other props
  return (
    prevProps.onCellClick === nextProps.onCellClick &&
    prevProps.cellSize === nextProps.cellSize &&
    prevProps.showGrid === nextProps.showGrid &&
    prevProps.className === nextProps.className &&
    prevProps.animateChanges === nextProps.animateChanges &&
    prevProps.readOnly === nextProps.readOnly
  );
};

export const GameBoard: React.FC<GameBoardProps> = memo(({
  board,
  onCellClick,
  cellSize = 12,
  showGrid = true,
  className = '',
  animateChanges = true,
  readOnly = false
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  
  // Create a memoized board key to trigger viewport reset when board dimensions change
  const boardKey = useMemo(() => 
    board ? `${board.width}-${board.height}` : 'no-board', 
    [board?.width, board?.height]
  );
  
  const { viewport, handlers, resetViewport } = useGameBoardViewport();
  const { zoom, pan, isDragging } = viewport;
  
  // Reset viewport when board key changes (replaces useEffect)
  const currentBoardKey = useRef(boardKey);
  if (currentBoardKey.current !== boardKey) {
    currentBoardKey.current = boardKey;
    resetViewport();
  }
  
  // Create lookup set for alive cells
  const aliveCellsSet = useAliveCells(board);
  
  // Calculate board dimensions
  const boardDimensions = useBoardDimensions(board, cellSize, zoom);

  // Keyboard handler using utility (replaces large switch statement)
  const handleKeyDown = useGameBoardKeyboardHandler({
    handleZoom: handlers.handleZoom,
    handlePan: handlers.handlePan,
    resetViewport
  });

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
      onKeyDown={handleKeyDown}
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
}, areEqual);

export default GameBoard;