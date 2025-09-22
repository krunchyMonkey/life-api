import { useEffect } from 'react';
import type { BoardDto } from '../types/api';
import { createDefaultBoard, STORAGE_KEYS } from '../constants/appConstants';

interface UseBoardInitializationProps {
  onBoardLoad: (board: BoardDto) => void;
}

export const useBoardInitialization = ({ onBoardLoad }: UseBoardInitializationProps) => {
  useEffect(() => {
    const initializeBoard = () => {
      // First, check URL for board data
      const urlParams = new URLSearchParams(window.location.search);
      const urlBoardData = urlParams.get('board');
      
      if (urlBoardData) {
        try {
          const board = JSON.parse(decodeURIComponent(urlBoardData)) as BoardDto;
          onBoardLoad(board);
          // Clean up URL
          window.history.replaceState({}, '', window.location.pathname);
          return; // Exit early, URL board takes precedence
        } catch (error) {
          console.warn('Failed to load board from URL:', error);
          // Continue to localStorage fallback
        }
      }

      // Second, try to load from localStorage
      const savedBoard = localStorage.getItem(STORAGE_KEYS.CURRENT_BOARD);
      if (savedBoard) {
        try {
          const board = JSON.parse(savedBoard) as BoardDto;
          onBoardLoad(board);
          return;
        } catch (error) {
          console.warn('Failed to load saved board:', error);
          // Continue to default board fallback
        }
      }

      // Finally, use default board
      onBoardLoad(createDefaultBoard());
    };

    initializeBoard();
  }, [onBoardLoad]);
};