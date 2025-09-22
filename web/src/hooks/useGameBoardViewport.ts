import { useState, useCallback, useEffect } from 'react';

export interface ViewportState {
  zoom: number;
  pan: { x: number; y: number };
  isDragging: boolean;
}

/**
 * Custom hook for managing GameBoard viewport interactions (zoom, pan, drag)
 */
export const useGameBoardViewport = () => {
  const [zoom, setZoom] = useState(1);
  const [pan, setPan] = useState({ x: 0, y: 0 });
  const [isDragging, setIsDragging] = useState(false);
  const [dragStart, setDragStart] = useState({ x: 0, y: 0 });

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

  // Handle programmatic zoom
  const handleZoom = useCallback((factor: number) => {
    setZoom(prevZoom => Math.max(0.1, Math.min(3, prevZoom * factor)));
  }, []);

  // Handle programmatic pan
  const handlePan = useCallback((delta: { x: number; y: number }) => {
    setPan(prevPan => ({ 
      x: prevPan.x + delta.x, 
      y: prevPan.y + delta.y 
    }));
  }, []);

  // Reset viewport when board dimensions change
  const resetViewport = useCallback(() => {
    setZoom(1);
    setPan({ x: 0, y: 0 });
  }, []);

  return {
    viewport: { zoom, pan, isDragging },
    handlers: {
      handleWheel,
      handleMouseDown,
      handleMouseMove,
      handleMouseUp,
      handleZoom,
      handlePan,
    },
    resetViewport,
  };
};

/**
 * Custom hook for GameBoard keyboard shortcuts
 */
export const useGameBoardKeyboard = (
  setZoom: (updater: (prev: number) => number) => void,
  setPan: (updater: (prev: { x: number; y: number }) => { x: number; y: number }) => void
) => {
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
          setZoom(() => 1);
          setPan(() => ({ x: 0, y: 0 }));
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
  }, [setZoom, setPan]);
};