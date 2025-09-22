import { useMemo, useCallback } from 'react';
import type { BoardDto } from '../../types/api';

/**
 * Types for keyboard handling
 */
export interface ViewportActions {
  handleZoom: (factor: number) => void;
  handlePan: (delta: { x: number; y: number }) => void;
  resetViewport: () => void;
}

/**
 * Keyboard command configuration
 */
export interface KeyboardCommand {
  key: string;
  action: (actions: ViewportActions) => void;
  preventDefault?: boolean;
}

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

  /**
   * Define keyboard commands for GameBoard navigation
   */
  static getKeyboardCommands(): KeyboardCommand[] {
    return [
      // Zoom commands
      {
        key: '+',
        action: (actions) => actions.handleZoom(1.2),
        preventDefault: true
      },
      {
        key: '=',
        action: (actions) => actions.handleZoom(1.2),
        preventDefault: true
      },
      {
        key: '-',
        action: (actions) => actions.handleZoom(0.8),
        preventDefault: true
      },
      // Reset command
      {
        key: '0',
        action: (actions) => actions.resetViewport(),
        preventDefault: true
      },
      // Pan commands
      {
        key: 'ArrowUp',
        action: (actions) => actions.handlePan({ x: 0, y: 20 }),
        preventDefault: true
      },
      {
        key: 'ArrowDown',
        action: (actions) => actions.handlePan({ x: 0, y: -20 }),
        preventDefault: true
      },
      {
        key: 'ArrowLeft',
        action: (actions) => actions.handlePan({ x: 20, y: 0 }),
        preventDefault: true
      },
      {
        key: 'ArrowRight',
        action: (actions) => actions.handlePan({ x: -20, y: 0 }),
        preventDefault: true
      }
    ];
  }

  /**
   * Handle keyboard event for GameBoard navigation
   */
  static handleKeyboardEvent(
    event: React.KeyboardEvent,
    actions: ViewportActions
  ): boolean {
    const commands = this.getKeyboardCommands();
    const command = commands.find(cmd => cmd.key === event.key);
    
    if (command) {
      if (command.preventDefault) {
        event.preventDefault();
      }
      command.action(actions);
      return true; // Event was handled
    }
    
    return false; // Event was not handled
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

/**
 * Custom hook for creating keyboard event handler
 */
export const useGameBoardKeyboardHandler = (actions: ViewportActions) => {
  return useCallback((event: React.KeyboardEvent) => {
    return BoardUtils.handleKeyboardEvent(event, actions);
  }, [actions]);
};