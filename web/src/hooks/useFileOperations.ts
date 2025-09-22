import { useCallback } from 'react';
import type { BoardDto } from '../types/api';

interface UseFileOperationsProps {
  currentBoard: BoardDto | null;
  onBoardLoad: (board: BoardDto) => void;
}

export const useFileOperations = ({ currentBoard, onBoardLoad }: UseFileOperationsProps) => {
  const handleExport = useCallback(() => {
    if (!currentBoard) return;

    const exportData = {
      board: currentBoard,
      metadata: {
        exported: new Date().toISOString(),
        version: '1.0',
        generator: 'Conway Game of Life Web App'
      }
    };

    const dataStr = JSON.stringify(exportData, null, 2);
    const dataBlob = new Blob([dataStr], { type: 'application/json' });
    const url = URL.createObjectURL(dataBlob);
    
    const link = document.createElement('a');
    link.href = url;
    link.download = `game-of-life-gen${currentBoard.generation}-${Date.now()}.json`;
    link.click();
    
    URL.revokeObjectURL(url);
  }, [currentBoard]);

  const handleImport = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (e) => {
      try {
        const data = JSON.parse(e.target?.result as string);
        const board = data.board || data; // Support both formats
        
        // Validate board structure
        if (board && typeof board.width === 'number' && typeof board.height === 'number' && Array.isArray(board.aliveCells)) {
          onBoardLoad(board as BoardDto);
        } else {
          alert('Invalid board file format');
        }
      } catch (error) {
        console.error('Failed to import board:', error);
        alert('Failed to import board file');
      }
    };
    reader.readAsText(file);
    
    // Clear the input
    event.target.value = '';
  }, [onBoardLoad]);

  const handleShare = useCallback(() => {
    if (!currentBoard) return;

    // Create a shareable URL with board data
    const boardData = encodeURIComponent(JSON.stringify(currentBoard));
    const shareUrl = `${window.location.origin}${window.location.pathname}?board=${boardData}`;
    
    if (navigator.clipboard && window.isSecureContext) {
      navigator.clipboard.writeText(shareUrl).then(() => {
        alert('Share URL copied to clipboard!');
      });
    } else {
      // Fallback for insecure contexts
      const textArea = document.createElement('textarea');
      textArea.value = shareUrl;
      document.body.appendChild(textArea);
      textArea.select();
      document.execCommand('copy');
      document.body.removeChild(textArea);
      alert('Share URL copied to clipboard!');
    }
  }, [currentBoard]);

  return {
    handleExport,
    handleImport,
    handleShare
  };
};