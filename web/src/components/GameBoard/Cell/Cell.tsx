import { memo, useCallback, useState } from 'react';

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

export const Cell = memo<CellProps>(({ 
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

  const [isHovered, setIsHovered] = useState(false);

  // Use Tailwind classes with dynamic styling
  const cellClasses = `
    game-cell
    ${isAlive ? 'alive' : 'dead'}
    ${!readOnly ? 'cursor-pointer' : 'cursor-default'}
    ${animated ? 'transition-all duration-200' : ''}
    ${isHovered && !readOnly ? 'scale-110' : ''}
  `.trim().replace(/\s+/g, ' ');

  return (
    <div
      className={cellClasses}
      style={{ 
        width: size, 
        height: size,
        border: showGrid ? '1px solid #d1d5db' : 'none'
      }}
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