import React, { memo, useCallback, useMemo, useRef, useState, useEffect } from 'react';
import type { BoardDto } from '../types/api';

interface GameBoardProps {
  board: BoardDto | null;
  onCellClick?: (x: number, y: number) => void;
  cellSize?: number;
  showGrid?: boolean;
  className?: string;
  animateChanges?: boolean;
  readOnly?: boolean;
}

interface CellProps {
  isAlive: boolean;
  x: number;
  y: number;
  size: number;
  onClick?: (x: number, y: number) => void;
  showGrid: boolean;
  animated: boolean;
  readOnly: boolean;
}

// Memoized Cell component for performance
const Cell = memo<CellProps>(({ 
  isAlive, 
  x, 
  y, 
  size, 
  onClick, 
  showGrid, 
  animated,
  readOnly 
}) => {
  const handleClick = useCallback(() => {
    if (!readOnly && onClick) {
      onClick(x, y);
    }
  }, [x, y, onClick, readOnly]);

  const cellStyle: React.CSSProperties = {
    width: size,
    height: size,
    backgroundColor: isAlive ? '#22c55e' : '#f3f4f6',
    border: showGrid ? '1px solid #d1d5db' : 'none',
    cursor: readOnly ? 'default' : 'pointer',
    transition: animated ? 'all 0.2s ease-in-out' : 'none',
  };

  const hoverStyle: React.CSSProperties = {
    ...cellStyle,
    backgroundColor: isAlive ? '#16a34a' : '#e5e7eb',
  };

  const [isHovered, setIsHovered] = useState(false);

  return (
    <div
      style={isHovered && !readOnly ? hoverStyle : cellStyle}
      onClick={handleClick}
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
      data-testid={`cell-${x}-${y}`}
      role={readOnly ? 'gridcell' : 'button'}
      aria-label={`Cell at ${x}, ${y} - ${isAlive ? 'alive' : 'dead'}`}
      tabIndex={readOnly ? -1 : 0}
      onKeyDown={(e) => {
        if (!readOnly && (e.key === 'Enter' || e.key === ' ')) {
          e.preventDefault();
          handleClick();
        }
      }}
    />
  );
});

Cell.displayName = 'Cell';

// Main GameBoard component
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
  const [zoom, setZoom] = useState(1);
  const [pan, setPan] = useState({ x: 0, y: 0 });
  const [isDragging, setIsDragging] = useState(false);
  const [dragStart, setDragStart] = useState({ x: 0, y: 0 });

  // Create a Set for fast lookup of alive cells
  const aliveCellsSet = useMemo(() => {
    if (!board?.aliveCells) return new Set<string>();
    return new Set(board.aliveCells.map(cell => `${cell.x},${cell.y}`));
  }, [board?.aliveCells]);

  // Handle zoom with mouse wheel
  const handleWheel = useCallback((e: React.WheelEvent) => {
    e.preventDefault();
    const delta = e.deltaY > 0 ? 0.9 : 1.1;
    setZoom(prevZoom => Math.max(0.1, Math.min(3, prevZoom * delta)));
  }, []);

  // Handle pan with mouse drag
  const handleMouseDown = useCallback((e: React.MouseEvent) => {
    if (e.button === 1 || (e.button === 0 && e.ctrlKey)) { // Middle mouse or Ctrl+click
      e.preventDefault();
      setIsDragging(true);
      setDragStart({ x: e.clientX - pan.x, y: e.clientY - pan.y });
    }
  }, [pan]);

  const handleMouseMove = useCallback((e: React.MouseEvent) => {
    if (isDragging) {
      setPan({
        x: e.clientX - dragStart.x,
        y: e.clientY - dragStart.y
      });
    }
  }, [isDragging, dragStart]);

  const handleMouseUp = useCallback(() => {
    setIsDragging(false);
  }, []);

  // Keyboard shortcuts for zoom and pan
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.target !== document.body) return; // Only when board has focus

      switch (e.key) {
        case '+':
        case '=':
          e.preventDefault();
          setZoom(prev => Math.min(3, prev * 1.2));
          break;
        case '-':
          e.preventDefault();
          setZoom(prev => Math.max(0.1, prev * 0.8));
          break;
        case '0':
          e.preventDefault();
          setZoom(1);
          setPan({ x: 0, y: 0 });
          break;
        case 'ArrowUp':
          e.preventDefault();
          setPan(prev => ({ ...prev, y: prev.y + 20 }));
          break;
        case 'ArrowDown':
          e.preventDefault();
          setPan(prev => ({ ...prev, y: prev.y - 20 }));
          break;
        case 'ArrowLeft':
          e.preventDefault();
          setPan(prev => ({ ...prev, x: prev.x + 20 }));
          break;
        case 'ArrowRight':
          e.preventDefault();
          setPan(prev => ({ ...prev, x: prev.x - 20 }));
          break;
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, []);

  // Reset zoom and pan when board changes
  useEffect(() => {
    setZoom(1);
    setPan({ x: 0, y: 0 });
  }, [board?.width, board?.height]);

  if (!board) {
    return (
      <div className={`flex items-center justify-center bg-gray-100 rounded-lg ${className}`}>
        <div className="text-center p-8">
          <div className="text-gray-400 text-4xl mb-2">🏁</div>
          <p className="text-gray-600">No board loaded</p>
          <p className="text-sm text-gray-500 mt-1">Upload a board or create a new one to get started</p>
        </div>
      </div>
    );
  }

  const boardWidth = board.width * cellSize * zoom;
  const boardHeight = board.height * cellSize * zoom;

  return (
    <div 
      className={`relative overflow-hidden bg-gray-100 rounded-lg border-2 border-gray-300 ${className}`}
      ref={containerRef}
      onWheel={handleWheel}
      onMouseDown={handleMouseDown}
      onMouseMove={handleMouseMove}
      onMouseUp={handleMouseUp}
      onMouseLeave={handleMouseUp}
      style={{ 
        cursor: isDragging ? 'grabbing' : 'grab',
        userSelect: 'none'
      }}
      tabIndex={0}
      role="grid"
      aria-label={`Game of Life board, ${board.width} by ${board.height}, generation ${board.generation}`}
    >
      {/* Board Info Overlay */}
      <div className="absolute top-2 left-2 bg-white bg-opacity-90 px-2 py-1 rounded text-xs font-mono">
        {board.width}×{board.height} | Gen: {board.generation} | Zoom: {(zoom * 100).toFixed(0)}%
      </div>

      {/* Controls Overlay */}
      <div className="absolute top-2 right-2 bg-white bg-opacity-90 px-2 py-1 rounded text-xs">
        <div>Scroll: Zoom | Ctrl+Drag: Pan</div>
        <div>+/-: Zoom | 0: Reset | Arrows: Pan</div>
      </div>

      {/* Game Board Grid */}
      <div 
        className="absolute"
        style={{
          transform: `translate(${pan.x}px, ${pan.y}px) scale(${zoom})`,
          transformOrigin: '0 0',
          width: boardWidth / zoom,
          height: boardHeight / zoom,
          display: 'grid',
          gridTemplateColumns: `repeat(${board.width}, ${cellSize}px)`,
          gridTemplateRows: `repeat(${board.height}, ${cellSize}px)`,
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

      {/* Performance hint for large boards */}
      {board.width * board.height > 10000 && (
        <div className="absolute bottom-2 left-2 bg-yellow-100 border border-yellow-400 px-2 py-1 rounded text-xs">
          ⚡ Large board detected. Consider disabling animations for better performance.
        </div>
      )}

      {/* Loading overlay */}
      {readOnly && (
        <div className="absolute inset-0 bg-black bg-opacity-10 flex items-center justify-center">
          <div className="bg-white px-3 py-1 rounded shadow text-sm">
            🔒 Read Only
          </div>
        </div>
      )}
    </div>
  );
};

export default GameBoard;