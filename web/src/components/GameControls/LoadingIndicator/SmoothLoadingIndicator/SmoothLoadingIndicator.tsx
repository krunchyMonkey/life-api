import React from 'react';
import { useDebouncedLoading } from '../../../../hooks/useDebouncedLoading';
import { spinnerSizes, textSizes } from './constants';

interface SmoothLoadingIndicatorProps {
  isLoading: boolean;
  /** Delay in ms before showing the indicator (default: 300ms) */
  showDelayMs?: number;
  /** Minimum time to show indicator once visible (default: 800ms) */
  minDisplayMs?: number;
  /** Custom loading text (default: "Computing...") */
  loadingText?: string;
  /** Size of the spinner: 'sm', 'md', 'lg' (default: 'md') */
  size?: 'sm' | 'md' | 'lg';
}

export const SmoothLoadingIndicator: React.FC<SmoothLoadingIndicatorProps> = ({ 
  isLoading, 
  showDelayMs = 300,
  minDisplayMs = 800,
  loadingText = "Computing...",
  size = 'md'
}) => {
  const shouldShow = useDebouncedLoading(isLoading, {
    showDelay: showDelayMs,
    minDisplay: minDisplayMs
  });

  if (!shouldShow) return null;

  return (
    <div className="flex items-center justify-center gap-2 py-2 animate-fadeIn">
      <div 
        className={`animate-spin rounded-full border-blue-500 border-t-transparent ${spinnerSizes[size]}`}
        role="status"
        aria-label="Loading"
      />
      <span className={`text-gray-600 ${textSizes[size]}`}>
        {loadingText}
      </span>
    </div>
  );
};