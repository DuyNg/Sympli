import { render, screen, fireEvent } from '@testing-library/react';
import { SearchForm } from './SearchForm';

test('calls onSearch with input values', () => {
  const onSearch = jest.fn();
  render(<SearchForm onSearch={onSearch} />);

  fireEvent.change(screen.getByPlaceholderText('Keyword'), { target: { value: 'test keyword' } });
  fireEvent.change(screen.getByPlaceholderText('URL'), { target: { value: 'http://example.com' } });
  fireEvent.change(screen.getByDisplayValue('All'), { target: { value: 'Google' } });

  fireEvent.click(screen.getByText('Search'));

  expect(onSearch).toHaveBeenCalledWith({
    keyword: 'test keyword',
    url: 'http://example.com',
    browser: 'Google',
  });
});
