import type { BoardDto } from '../types/api';

// Query Client Configuration
export const QUERY_CLIENT_CONFIG = {
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
    },
    mutations: {
      retry: (failureCount: number, error: any) => {
        // Don't retry database concurrency errors
        if (error?.message?.includes('DbUpdateException') || 
            error?.message?.includes('EntityFrameworkCore') ||
            error?.message?.includes('entity changes')) {
          return false;
        }
        // Retry network errors up to 2 times
        return failureCount < 2;
      },
      retryDelay: (attemptIndex: number) => Math.min(1000 * 2 ** attemptIndex, 3000), // Exponential backoff, max 3s
    },
  },
} as const;

// Game Configuration
export const GAME_CONFIG = {
  maxHistory: 100,
  defaultSpeed: 5,
  autoSave: true,
} as const;

// Default board for initialization
export const createDefaultBoard = (): BoardDto => ({
  width: 50,
  height: 30,
  aliveCells: [
    // Simple glider
    { x: 1, y: 2 },
    { x: 2, y: 3 },
    { x: 3, y: 1 },
    { x: 3, y: 2 },
    { x: 3, y: 3 }
  ],
  generation: 0
});

// Local Storage Keys
export const STORAGE_KEYS = {
  CURRENT_BOARD: 'gameOfLife_currentBoard'
} as const;