import React, { useState, useEffect } from 'react';

interface LoadingIndicatorProps {
  isLoading: boolean;
  /** Delay in ms before showing the indicator (default: 300ms) */
  delayMs?: number;
  /** Minimum time to show indicator once visible (default: 800ms) */
  minDisplayMs?: number;
}

export const LoadingIndicator: React.FC<LoadingIndicatorProps> = ({ 
  isLoading, 
  delayMs = 300,
  minDisplayMs = 800 
}) => {
  const [shouldShow, setShouldShow] = useState(false);
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    let delayTimer: NodeJS.Timeout | undefined;
    let minDisplayTimer: NodeJS.Timeout | undefined;

    if (isLoading) {
      // Only show after delay to prevent flicker for quick operations
      delayTimer = setTimeout(() => {
        setShouldShow(true);
        setIsVisible(true);
      }, delayMs);
    } else {
      // Clear delay timer if loading stops before delay
      if (delayTimer) clearTimeout(delayTimer);
      
      if (shouldShow) {
        // If currently showing, ensure minimum display time
        minDisplayTimer = setTimeout(() => {
          setShouldShow(false);
          // Fade out transition
          setTimeout(() => setIsVisible(false), 150);
        }, minDisplayMs);
      }
    }

    return () => {
      if (delayTimer) clearTimeout(delayTimer);
      if (minDisplayTimer) clearTimeout(minDisplayTimer);
    };
  }, [isLoading, delayMs, minDisplayMs, shouldShow]);

  if (!isVisible) return null;

  return (
    <div 
      className={`flex items-center justify-center gap-2 py-2 transition-opacity duration-150 ${
        shouldShow ? 'opacity-100' : 'opacity-0'
      }`}
    >
      <div className="animate-spin rounded-full h-4 w-4 border-2 border-blue-500 border-t-transparent"></div>
      <span className="text-sm text-gray-600">Computing...</span>
    </div>
  );
};