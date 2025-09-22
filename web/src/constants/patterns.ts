import type { ConwayPattern, Position } from '../types/api';

// Famous Conway's Game of Life patterns
export const CONWAY_PATTERNS: Record<string, ConwayPattern[]> = {
  stillLifes: [
    {
      name: 'Block',
      positions: [
        { x: 0, y: 0 }, { x: 1, y: 0 },
        { x: 0, y: 1 }, { x: 1, y: 1 }
      ],
      description: 'The simplest still life - a 2x2 square',
      discoverer: 'John Conway',
      year: 1970
    },
    {
      name: 'Bee-hive',
      positions: [
        { x: 1, y: 0 }, { x: 2, y: 0 },
        { x: 0, y: 1 }, { x: 3, y: 1 },
        { x: 1, y: 2 }, { x: 2, y: 2 }
      ],
      description: 'A hexagonal still life pattern',
      discoverer: 'John Conway',
      year: 1970
    },
    {
      name: 'Loaf',
      positions: [
        { x: 1, y: 0 }, { x: 2, y: 0 },
        { x: 0, y: 1 }, { x: 3, y: 1 },
        { x: 1, y: 2 }, { x: 3, y: 2 },
        { x: 2, y: 3 }
      ],
      description: 'A bread loaf-shaped still life',
      discoverer: 'John Conway',
      year: 1970
    }
  ],
  
  oscillators: [
    {
      name: 'Blinker',
      positions: [
        { x: 0, y: 1 }, { x: 1, y: 1 }, { x: 2, y: 1 }
      ],
      description: 'A period-2 oscillator - alternates between horizontal and vertical',
      discoverer: 'John Conway',
      year: 1970
    },
    {
      name: 'Toad',
      positions: [
        { x: 1, y: 1 }, { x: 2, y: 1 }, { x: 3, y: 1 },
        { x: 0, y: 2 }, { x: 1, y: 2 }, { x: 2, y: 2 }
      ],
      description: 'A period-2 oscillator',
      discoverer: 'John Conway',
      year: 1970
    },
    {
      name: 'Beacon',
      positions: [
        { x: 0, y: 0 }, { x: 1, y: 0 },
        { x: 0, y: 1 },
        { x: 3, y: 2 },
        { x: 2, y: 3 }, { x: 3, y: 3 }
      ],
      description: 'A period-2 oscillator',
      discoverer: 'John Conway',
      year: 1970
    },
    {
      name: 'Pulsar',
      positions: [
        // Top part
        { x: 2, y: 0 }, { x: 3, y: 0 }, { x: 4, y: 0 }, { x: 8, y: 0 }, { x: 9, y: 0 }, { x: 10, y: 0 },
        { x: 0, y: 2 }, { x: 5, y: 2 }, { x: 7, y: 2 }, { x: 12, y: 2 },
        { x: 0, y: 3 }, { x: 5, y: 3 }, { x: 7, y: 3 }, { x: 12, y: 3 },
        { x: 0, y: 4 }, { x: 5, y: 4 }, { x: 7, y: 4 }, { x: 12, y: 4 },
        { x: 2, y: 5 }, { x: 3, y: 5 }, { x: 4, y: 5 }, { x: 8, y: 5 }, { x: 9, y: 5 }, { x: 10, y: 5 },
        // Bottom part (mirrored)
        { x: 2, y: 7 }, { x: 3, y: 7 }, { x: 4, y: 7 }, { x: 8, y: 7 }, { x: 9, y: 7 }, { x: 10, y: 7 },
        { x: 0, y: 8 }, { x: 5, y: 8 }, { x: 7, y: 8 }, { x: 12, y: 8 },
        { x: 0, y: 9 }, { x: 5, y: 9 }, { x: 7, y: 9 }, { x: 12, y: 9 },
        { x: 0, y: 10 }, { x: 5, y: 10 }, { x: 7, y: 10 }, { x: 12, y: 10 },
        { x: 2, y: 12 }, { x: 3, y: 12 }, { x: 4, y: 12 }, { x: 8, y: 12 }, { x: 9, y: 12 }, { x: 10, y: 12 }
      ],
      description: 'A period-3 oscillator with complex behavior',
      discoverer: 'John Conway',
      year: 1970
    }
  ],
  
  spaceships: [
    {
      name: 'Glider',
      positions: [
        { x: 1, y: 0 },
        { x: 2, y: 1 },
        { x: 0, y: 2 }, { x: 1, y: 2 }, { x: 2, y: 2 }
      ],
      description: 'The smallest spaceship - travels diagonally every 4 generations',
      discoverer: 'Richard K. Guy',
      year: 1970
    },
    {
      name: 'Light-weight Spaceship (LWSS)',
      positions: [
        { x: 0, y: 0 }, { x: 3, y: 0 },
        { x: 4, y: 1 },
        { x: 0, y: 2 }, { x: 4, y: 2 },
        { x: 1, y: 3 }, { x: 2, y: 3 }, { x: 3, y: 3 }, { x: 4, y: 3 }
      ],
      description: 'A period-4 spaceship that moves horizontally',
      discoverer: 'John Conway',
      year: 1970
    },
    {
      name: 'Middle-weight Spaceship (MWSS)',
      positions: [
        { x: 2, y: 0 },
        { x: 0, y: 1 }, { x: 4, y: 1 },
        { x: 5, y: 2 },
        { x: 0, y: 3 }, { x: 5, y: 3 },
        { x: 1, y: 4 }, { x: 2, y: 4 }, { x: 3, y: 4 }, { x: 4, y: 4 }, { x: 5, y: 4 }
      ],
      description: 'A period-4 spaceship, larger than LWSS',
      discoverer: 'John Conway',
      year: 1970
    },
    {
      name: 'Heavy-weight Spaceship (HWSS)',
      positions: [
        { x: 2, y: 0 }, { x: 3, y: 0 },
        { x: 0, y: 1 }, { x: 5, y: 1 },
        { x: 6, y: 2 },
        { x: 0, y: 3 }, { x: 6, y: 3 },
        { x: 1, y: 4 }, { x: 2, y: 4 }, { x: 3, y: 4 }, { x: 4, y: 4 }, { x: 5, y: 4 }, { x: 6, y: 4 }
      ],
      description: 'The largest standard spaceship',
      discoverer: 'John Conway',
      year: 1970
    }
  ],
  
  guns: [
    {
      name: 'Gosper Glider Gun',
      positions: [
        // Left square
        { x: 1, y: 5 }, { x: 2, y: 5 },
        { x: 1, y: 6 }, { x: 2, y: 6 },
        // Left circle
        { x: 11, y: 3 }, { x: 11, y: 4 }, { x: 11, y: 5 },
        { x: 12, y: 2 }, { x: 12, y: 6 },
        { x: 13, y: 1 }, { x: 13, y: 7 },
        { x: 14, y: 1 }, { x: 14, y: 7 },
        { x: 15, y: 4 },
        { x: 16, y: 2 }, { x: 16, y: 6 },
        { x: 17, y: 3 }, { x: 17, y: 4 }, { x: 17, y: 5 },
        { x: 18, y: 4 },
        // Right circle
        { x: 21, y: 1 }, { x: 21, y: 2 }, { x: 21, y: 3 },
        { x: 22, y: 1 }, { x: 22, y: 2 }, { x: 22, y: 3 },
        { x: 23, y: 0 }, { x: 23, y: 4 },
        { x: 25, y: 0 }, { x: 25, y: 1 }, { x: 25, y: 4 }, { x: 25, y: 5 },
        // Right square
        { x: 35, y: 2 }, { x: 36, y: 2 },
        { x: 35, y: 3 }, { x: 36, y: 3 }
      ],
      description: 'The first discovered gun - produces gliders every 30 generations',
      discoverer: 'Bill Gosper',
      year: 1970
    }
  ],
  
  methuselahs: [
    {
      name: 'R-pentomino',
      positions: [
        { x: 1, y: 0 }, { x: 2, y: 0 },
        { x: 0, y: 1 }, { x: 1, y: 1 },
        { x: 1, y: 2 }
      ],
      description: 'A methuselah that stabilizes after 1103 generations',
      discoverer: 'John Conway',
      year: 1970
    },
    {
      name: 'Diehard',
      positions: [
        { x: 6, y: 0 },
        { x: 0, y: 1 }, { x: 1, y: 1 },
        { x: 1, y: 2 }, { x: 5, y: 2 }, { x: 6, y: 2 }, { x: 7, y: 2 }
      ],
      description: 'Dies completely after 130 generations',
      discoverer: 'Bill Gosper',
      year: 1970
    },
    {
      name: 'Acorn',
      positions: [
        { x: 1, y: 0 },
        { x: 3, y: 1 },
        { x: 0, y: 2 }, { x: 1, y: 2 }, { x: 4, y: 2 }, { x: 5, y: 2 }, { x: 6, y: 2 }
      ],
      description: 'Grows for over 5000 generations before stabilizing',
      discoverer: 'Charles Corderman',
      year: 1971
    }
  ]
};

// Get all patterns as a flat array
export const getAllPatterns = (): ConwayPattern[] => {
  return Object.values(CONWAY_PATTERNS).flat();
};

// Get patterns by category
export const getPatternsByCategory = (category: keyof typeof CONWAY_PATTERNS): ConwayPattern[] => {
  return CONWAY_PATTERNS[category] || [];
};

// Utility function to get pattern dimensions
export const getPatternDimensions = (positions: Position[]) => {
  if (positions.length === 0) return { width: 0, height: 0 };
  
  const minX = Math.min(...positions.map(p => p.x));
  const maxX = Math.max(...positions.map(p => p.x));
  const minY = Math.min(...positions.map(p => p.y));
  const maxY = Math.max(...positions.map(p => p.y));
  
  return {
    width: maxX - minX + 1,
    height: maxY - minY + 1,
    minX,
    minY
  };
};

// Center a pattern in a given board size
export const centerPattern = (positions: Position[], boardWidth: number, boardHeight: number): Position[] => {
  const { width, height, minX = 0, minY = 0 } = getPatternDimensions(positions);
  
  const offsetX = Math.floor((boardWidth - width) / 2) - minX;
  const offsetY = Math.floor((boardHeight - height) / 2) - minY;
  
  return positions.map(pos => ({
    x: pos.x + offsetX,
    y: pos.y + offsetY
  }));
};