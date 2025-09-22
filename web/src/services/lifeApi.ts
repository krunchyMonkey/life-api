import axios, { AxiosError } from 'axios';
import type { AxiosInstance } from 'axios';
import type { 
  ApiBoardDto, 
  UploadRequest,
  UploadResponse,
  NextRequest, 
  NAheadRequest, 
  FinalRequest,
  FinalResponse,
  ErrorResponse 
} from '../types/api';

// API Configuration
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '';

// Custom error class for API errors
export class ApiError extends Error {
  status: number;
  title: string;
  detail: string;
  instance?: string;

  constructor(
    status: number,
    title: string,
    detail: string,
    instance?: string
  ) {
    super(`${title}: ${detail}`);
    this.name = 'ApiError';
    this.status = status;
    this.title = title;
    this.detail = detail;
    this.instance = instance;
  }
}

// API Client class
class LifeApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      timeout: 30000, // 30 second timeout
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      }
    });

    // Request interceptor for logging
    this.client.interceptors.request.use(
      (config) => {
        console.log(`🚀 API Request: ${config.method?.toUpperCase()} ${config.url}`);
        return config;
      },
      (error) => {
        console.error('❌ Request Error:', error);
        return Promise.reject(error);
      }
    );

    // Response interceptor for error handling
    this.client.interceptors.response.use(
      (response) => {
        console.log(`✅ API Response: ${response.config.method?.toUpperCase()} ${response.config.url} - ${response.status}`);
        return response;
      },
      (error: AxiosError) => {
        console.error('❌ API Error:', error.response?.status, error.message);
        return Promise.reject(this.handleApiError(error));
      }
    );
  }

  private handleApiError(error: AxiosError): ApiError {
    if (error.response) {
      // Server responded with error status
      const errorData = error.response.data as ErrorResponse;
      
      return new ApiError(
        error.response.status,
        errorData.title || 'Server Error',
        errorData.detail || error.message,
        errorData.instance
      );
    } else if (error.request) {
      // Request was made but no response received
      return new ApiError(
        0,
        'Network Error',
        'No response received from server. Please check your connection.',
      );
    } else {
      // Something else happened
      return new ApiError(
        0,
        'Request Error',
        error.message || 'An unknown error occurred'
      );
    }
  }

  /**
   * Upload a board to the API and get a board ID
   */
  async uploadBoard(width: number, height: number, alive: number[][]): Promise<UploadResponse> {
    const request: UploadRequest = { width, height, alive };
    
    const response = await this.client.post<UploadResponse>('/api/v1/boards/upload', request);
    return response.data;
  }

  /**
   * Get the next generation of a board by board ID
   */
  async getNextGeneration(boardId: string): Promise<ApiBoardDto> {
    const request: NextRequest = { boardId };
    
    const response = await this.client.post<ApiBoardDto>('/api/v1/boards/next', request);
    return response.data;
  }

  /**
   * Advance a board by N generations
   */
  async advanceGenerations(boardId: string, n: number): Promise<ApiBoardDto> {
    const request: NAheadRequest = { boardId, n };
    
    const response = await this.client.post<ApiBoardDto>('/api/v1/boards/n-ahead', request);
    return response.data;
  }

  /**
   * Get the final stable state of a board
   */
  async getFinalState(boardId: string): Promise<FinalResponse> {
    const request: FinalRequest = { boardId };
    
    const response = await this.client.post<FinalResponse>('/api/v1/boards/final', request);
    return response.data;
  }

  /**
   * Check API health
   */
  async healthCheck(): Promise<boolean> {
    try {
      // Try to get a simple response from the API
      // This might need to be adjusted based on your API's health endpoint
      await this.client.get('/health');
      return true;
    } catch {
      return false;
    }
  }

  /**
   * Get API base URL for debugging
   */
  getBaseUrl(): string {
    return API_BASE_URL;
  }
}

// Create and export a singleton instance
export const apiClient = new LifeApiClient();

// Export the class for testing purposes
export { LifeApiClient };