import { useMutation, useQuery } from '@tanstack/react-query';
import { gameService } from '../services/gameService';
import { apiClient, ApiError } from '../services/lifeApi';
import type { BoardDto } from '../types/api';

// Query keys for React Query
export const queryKeys = {
  health: ['api', 'health'] as const,
  board: (board: BoardDto) => ['board', board] as const,
};

// Custom hooks for API operations

/**
 * Hook for getting the next generation
 */
export const useNextGeneration = () => {
  return useMutation({
    mutationFn: (board: BoardDto) => 
      gameService.getNextGeneration(board.aliveCells, board.width, board.height),
    onSuccess: (positions) => {
      console.log('✅ Next generation computed:', positions);
    },
    onError: (error: ApiError) => {
      console.error('❌ Failed to get next generation:', error);
    },
  });
};

/**
 * Hook for advancing N generations
 */
export const useAdvanceGenerations = () => {
  return useMutation({
    mutationFn: ({ board, n }: { board: BoardDto; n: number }) => 
      gameService.advanceGenerations(board.aliveCells, board.width, board.height, n),
    onSuccess: (positions, variables) => {
      console.log(`✅ Advanced ${variables.n} generations:`, positions);
    },
    onError: (error: ApiError) => {
      console.error('❌ Failed to advance generations:', error);
    },
  });
};

/**
 * Hook for getting the final state
 */
export const useFinalState = () => {
  return useMutation({
    mutationFn: (board: BoardDto) => 
      gameService.getFinalState(board.aliveCells, board.width, board.height),
    onSuccess: (data) => {
      console.log('✅ Final state computed:', data);
    },
    onError: (error: ApiError) => {
      console.error('❌ Failed to get final state:', error);
    },
  });
};

/**
 * Hook for checking API health
 */
export const useApiHealth = () => {
  return useQuery({
    queryKey: queryKeys.health,
    queryFn: () => apiClient.healthCheck(),
    refetchInterval: 30000, // Check every 30 seconds
    retry: 3,
    staleTime: 10000, // Consider data stale after 10 seconds
  });
};