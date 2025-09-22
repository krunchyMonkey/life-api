import type { BoardDto } from '../../../types/api';

interface BoardInfoOverlayProps {
  board: BoardDto;
  zoom: number;
}

export const BoardInfoOverlay: React.FC<BoardInfoOverlayProps> = ({ board, zoom }) => (
  <div className="absolute top-2 left-2 bg-white/90 px-3 py-1 rounded-md text-xs font-mono shadow-sm border border-gray-200">
    {board.width}×{board.height} | Gen: {board.generation} | Zoom: {(zoom * 100).toFixed(0)}%
  </div>
);

export const BoardControlsOverlay: React.FC = () => (
  <div className="absolute top-2 right-2 bg-white/90 px-3 py-1 rounded-md text-xs shadow-sm border border-gray-200">
    <div className="font-medium">Scroll: Zoom | Ctrl+Drag: Pan</div>
    <div className="text-gray-600">+/-: Zoom | 0: Reset | Arrows: Pan</div>
  </div>
);

interface PerformanceHintOverlayProps {
  board: BoardDto;
}

export const PerformanceHintOverlay: React.FC<PerformanceHintOverlayProps> = ({ board }) => {
  if (board.width * board.height <= 10000) return null;

  return (
    <div className="absolute bottom-2 left-2 bg-amber-50 border border-amber-200 px-3 py-1 rounded-md text-xs text-amber-800">
      ⚡ Large board detected. Consider disabling animations for better performance.
    </div>
  );
};

interface ReadOnlyOverlayProps {
  readOnly: boolean;
}

export const ReadOnlyOverlay: React.FC<ReadOnlyOverlayProps> = ({ readOnly }) => {
  if (!readOnly) return null;

  return (
    <div className="absolute inset-0 bg-black/10 flex items-center justify-center">
      <div className="bg-white px-3 py-2 rounded-md shadow-md text-sm font-medium">
        🔒 Read Only
      </div>
    </div>
  );
};