import { useCallback } from 'react';
import type { BoardDto, ConwayPattern, Position } from '../types/api';

interface UsePatternOperationsProps {
  currentBoard: BoardDto | null;
  onBoardUpdate: (board: BoardDto) => void;
  onPatternLibraryClose: () => void;
}

export const usePatternOperations = ({
  currentBoard,
  onBoardUpdate,
  onPatternLibraryClose
}: UsePatternOperationsProps) => {
  // Handle pattern selection from library
  const handlePatternSelect = useCallback((pattern: ConwayPattern) => {
    // This could be extended to do something with the selected pattern
    // For now, we'll just pass it through to the parent component
    return pattern;
  }, []);

  // Handle pattern insertion
  const handlePatternInsert = useCallback((positions: Position[]) => {
    if (!currentBoard) return;

    // Add pattern positions to current alive cells
    const existingCells = new Set(
      currentBoard.aliveCells.map(cell => `${cell.x},${cell.y}`)
    );

    const newAliveCells = [...currentBoard.aliveCells];
    
    positions.forEach(pos => {
      const key = `${pos.x},${pos.y}`;
      if (!existingCells.has(key) && 
          pos.x >= 0 && pos.x < currentBoard.width &&
          pos.y >= 0 && pos.y < currentBoard.height) {
        newAliveCells.push(pos);
      }
    });

    const updatedBoard: BoardDto = {
      ...currentBoard,
      aliveCells: newAliveCells
    };

    onBoardUpdate(updatedBoard);
    onPatternLibraryClose();
  }, [currentBoard, onBoardUpdate, onPatternLibraryClose]);

  return {
    handlePatternSelect,
    handlePatternInsert
  };
};