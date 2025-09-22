import { apiClient } from './lifeApi';
import type { Position } from '../types/api';

/**
 * Service layer that bridges between the frontend board representation 
 * and the API's board ID system
 */
export class GameService {
  private boardIdCache = new Map<string, string>();

  /**
   * Convert position array to coordinate array format expected by API
   */
  private positionsToCoordinates(positions: Position[]): number[][] {
    return positions.map(pos => [pos.x, pos.y]);
  }

  /**
   * Convert coordinate array format from API to position array
   */
  private coordinatesToPositions(coordinates: number[][]): Position[] {
    return coordinates.map(([x, y]) => ({ x, y }));
  }

  /**
   * Create cache key for board state
   */
  private getBoardKey(positions: Position[], width: number, height: number): string {
    const sorted = positions.slice().sort((a, b) => a.x - b.x || a.y - b.y);
    return `${width}x${height}:${JSON.stringify(sorted)}`;
  }

  /**
   * Get next generation for a board
   */
  async getNextGeneration(positions: Position[], width: number, height: number): Promise<Position[]> {
    try {
      const boardKey = this.getBoardKey(positions, width, height);
      let boardId = this.boardIdCache.get(boardKey);

      // Upload board if not cached
      if (!boardId) {
        const coordinates = this.positionsToCoordinates(positions);
        const uploadResponse = await apiClient.uploadBoard(width, height, coordinates);
        boardId = uploadResponse.boardId;
        this.boardIdCache.set(boardKey, boardId);
      }

      // Get next generation
      const nextBoard = await apiClient.getNextGeneration(boardId);
      return this.coordinatesToPositions(nextBoard.alive);
    } catch (error) {
      console.error('Failed to get next generation:', error);
      throw error;
    }
  }

  /**
   * Advance N generations
   */
  async advanceGenerations(positions: Position[], width: number, height: number, n: number): Promise<Position[]> {
    try {
      const boardKey = this.getBoardKey(positions, width, height);
      let boardId = this.boardIdCache.get(boardKey);

      // Upload board if not cached
      if (!boardId) {
        const coordinates = this.positionsToCoordinates(positions);
        const uploadResponse = await apiClient.uploadBoard(width, height, coordinates);
        boardId = uploadResponse.boardId;
        this.boardIdCache.set(boardKey, boardId);
      }

      // Advance N generations
      const nextBoard = await apiClient.advanceGenerations(boardId, n);
      return this.coordinatesToPositions(nextBoard.alive);
    } catch (error) {
      console.error('Failed to advance generations:', error);
      throw error;
    }
  }

  /**
   * Get final stable state
   */
  async getFinalState(positions: Position[], width: number, height: number): Promise<{
    positions: Position[];
    stable: boolean;
    cyclic: boolean;
    iterations: number;
    cycleLength?: number;
  }> {
    try {
      const boardKey = this.getBoardKey(positions, width, height);
      let boardId = this.boardIdCache.get(boardKey);

      // Upload board if not cached
      if (!boardId) {
        const coordinates = this.positionsToCoordinates(positions);
        const uploadResponse = await apiClient.uploadBoard(width, height, coordinates);
        boardId = uploadResponse.boardId;
        this.boardIdCache.set(boardKey, boardId);
      }

      // Get final state
      const finalResponse = await apiClient.getFinalState(boardId);
      return {
        positions: this.coordinatesToPositions(finalResponse.board.alive),
        stable: finalResponse.stable,
        cyclic: finalResponse.cyclic,
        iterations: finalResponse.iterations,
        cycleLength: finalResponse.cycleLength
      };
    } catch (error) {
      console.error('Failed to get final state:', error);
      throw error;
    }
  }

  /**
   * Clear the board ID cache
   */
  clearCache(): void {
    this.boardIdCache.clear();
  }
}

export const gameService = new GameService();