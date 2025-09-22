import { useState, useEffect, useRef } from 'react';

/**
 * Custom hook to manage smooth loading states
 * Prevents flickering by debouncing show/hide with configurable delays
 */
export const useDebouncedLoading = (
  isLoading: boolean, 
  options: {
    /** Delay before showing indicator (default: 300ms) */
    showDelay?: number;
    /** Minimum time to display once shown (default: 800ms) */
    minDisplay?: number;
  } = {}
) => {
  const { showDelay = 300, minDisplay = 800 } = options;
  const [shouldShow, setShouldShow] = useState(false);
  const timersRef = useRef<{
    showTimer?: NodeJS.Timeout;
    hideTimer?: NodeJS.Timeout;
  }>({});

  useEffect(() => {
    const { current: timers } = timersRef;

    // Clear any existing timers
    if (timers.showTimer) clearTimeout(timers.showTimer);
    if (timers.hideTimer) clearTimeout(timers.hideTimer);

    if (isLoading) {
      if (!shouldShow) {
        // Show after delay to prevent flicker
        timers.showTimer = setTimeout(() => {
          setShouldShow(true);
        }, showDelay);
      }
    } else {
      if (shouldShow) {
        // Hide after minimum display time
        timers.hideTimer = setTimeout(() => {
          setShouldShow(false);
        }, minDisplay);
      }
    }

    return () => {
      if (timers.showTimer) clearTimeout(timers.showTimer);
      if (timers.hideTimer) clearTimeout(timers.hideTimer);
    };
  }, [isLoading, shouldShow, showDelay, minDisplay]);

  return shouldShow;
};