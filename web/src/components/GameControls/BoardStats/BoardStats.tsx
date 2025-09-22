import React from 'react';
import type { BoardDto } from '../../../types/api';

interface BoardStatsProps {
  board: BoardDto;
}

export const BoardStats: React.FC<BoardStatsProps> = ({ board }) => {
  const aliveCellsCount = board.aliveCells.length;
  const totalCells = board.width * board.height;
  const density = ((aliveCellsCount / totalCells) * 100).toFixed(1);

  return (
    <div className="stats-grid">
      <div className="stat-item">
        <div className="stat-value">{board.generation}</div>
        <div className="stat-label">Generation</div>
      </div>
      <div className="stat-item">
        <div className="stat-value">{board.width}×{board.height}</div>
        <div className="stat-label">Size</div>
      </div>
      <div className="stat-item">
        <div className="stat-value">{aliveCellsCount}</div>
        <div className="stat-label">Alive Cells</div>
      </div>
      <div className="stat-item">
        <div className="stat-value">{density}%</div>
        <div className="stat-label">Density</div>
      </div>
    </div>
  );
};