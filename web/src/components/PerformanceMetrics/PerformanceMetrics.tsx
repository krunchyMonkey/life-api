import React, { memo } from 'react';

interface PerformanceData {
  generationTime: number;
  generationsPerSecond: number;
}

interface PerformanceMetricsProps {
  performanceMetrics: PerformanceData | null;
}

export const PerformanceMetrics: React.FC<PerformanceMetricsProps> = memo(({ performanceMetrics }) => {
  if (!performanceMetrics) return null;

  return (
    <div className="status-panel">
      <h3 className="font-semibold mb-2">Performance</h3>
      <div className="space-y-1 text-sm">
        <div className="flex justify-between">
          <span>Generation Time:</span>
          <span className="metric-value">{performanceMetrics.generationTime.toFixed(1)}ms</span>
        </div>
        <div className="flex justify-between">
          <span>Speed:</span>
          <span className="metric-value">{performanceMetrics.generationsPerSecond.toFixed(1)}/s</span>
        </div>
      </div>
    </div>
  );
});