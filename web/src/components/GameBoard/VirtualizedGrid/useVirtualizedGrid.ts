import { useMemo } from 'react';
import type { BoardDto } from '../../../types/api';

interface VirtualizedGridOptions {
  board: BoardDto;
  cellSize: number;
  zoom: number;
  pan: { x: number; y: number };
  containerWidth?: number;
  containerHeight?: number;
}

/**
 * Hook for virtualizing large game boards to improve performance
 * Only renders cells that are visible in the viewport
 */
export const useVirtualizedGrid = ({
  board,
  cellSize,
  zoom,
  pan,
  containerWidth = 800,
  containerHeight = 600,
}: VirtualizedGridOptions) => {
  const visibleCells = useMemo(() => {
    const scaledCellSize = cellSize * zoom;
    
    // Calculate visible bounds
    const startX = Math.max(0, Math.floor(-pan.x / scaledCellSize));
    const endX = Math.min(board.width - 1, Math.floor((-pan.x + containerWidth) / scaledCellSize));
    const startY = Math.max(0, Math.floor(-pan.y / scaledCellSize));
    const endY = Math.min(board.height - 1, Math.floor((-pan.y + containerHeight) / scaledCellSize));
    
    const cells: Array<{ x: number; y: number; key: string }> = [];
    
    for (let y = startY; y <= endY; y++) {
      for (let x = startX; x <= endX; x++) {
        cells.push({ x, y, key: `${x}-${y}` });
      }
    }
    
    return cells;
  }, [board.width, board.height, cellSize, zoom, pan.x, pan.y, containerWidth, containerHeight]);

  const shouldVirtualize = board.width * board.height > 10000;
  
  return {
    visibleCells,
    shouldVirtualize,
    totalCells: board.width * board.height,
    visibleCellCount: visibleCells.length,
  };
};