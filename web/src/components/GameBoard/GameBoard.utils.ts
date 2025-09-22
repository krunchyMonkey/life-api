import { useMemo } from 'react';
import type { BoardDto } from '../../types/api';

/**
 * Utility functions for board operations
 */
export class BoardUtils {
  /**
   * Create a Set for fast lookup of alive cells
   */
  static createAliveCellsSet(board: BoardDto | null): Set<string> {
    if (!board?.aliveCells) return new Set<string>();
    return new Set(board.aliveCells.map(cell => `${cell.x},${cell.y}`));
  }

  /**
   * Calculate board dimensions in pixels
   */
  static calculateBoardDimensions(board: BoardDto, cellSize: number, zoom: number) {
    return {
      width: board.width * cellSize * zoom,
      height: board.height * cellSize * zoom
    };
  }

  /**
   * Generate grid template for CSS Grid
   */
  static getGridTemplate(board: BoardDto, cellSize: number) {
    return {
      gridTemplateColumns: `repeat(${board.width}, ${cellSize}px)`,
      gridTemplateRows: `repeat(${board.height}, ${cellSize}px)`,
    };
  }

  /**
   * Check if board is considered large for performance warnings
   */
  static isLargeBoard(board: BoardDto): boolean {
    return board.width * board.height > 10000;
  }
}

/**
 * Custom hook for managing alive cells lookup
 */
export const useAliveCells = (board: BoardDto | null) => {
  return useMemo(() => BoardUtils.createAliveCellsSet(board), [board?.aliveCells]);
};

/**
 * Custom hook for board dimensions calculations
 */
export const useBoardDimensions = (board: BoardDto | null, cellSize: number, zoom: number) => {
  return useMemo(() => {
    if (!board) return null;
    return BoardUtils.calculateBoardDimensions(board, cellSize, zoom);
  }, [board, cellSize, zoom]);
};