import React from 'react';
import './ResultsTable.scss';

interface Props {
  results: { browser: string; positions: number[] }[];
}

export const ResultsTable: React.FC<Props> = ({ results }) => {
  if (results.length === 0) return null;

  return (
    <table className="results-table">
      <thead>
        <tr>
          <th>Browser</th>
          <th>Positions</th>
        </tr>
      </thead>
      <tbody>
        {results.map((result, index) => (
          <tr key={index}>
            <td>{result.browser}</td>
            <td>{result.positions.join(', ')}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
};