import { render, screen } from '@testing-library/react';
import { ResultsTable } from './ResultsTable';

test('renders table with results', () => {
  const results = [
    { browser: 'Google', positions: '1, 3, 5' },
    { browser: 'Bing', positions: '2, 4' },
  ];

  render(<ResultsTable results={results} />);

  expect(screen.getByText('Google')).toBeInTheDocument();
  expect(screen.getByText('1, 3, 5')).toBeInTheDocument();
  expect(screen.getByText('Bing')).toBeInTheDocument();
  expect(screen.getByText('2, 4')).toBeInTheDocument();
});
