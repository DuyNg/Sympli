import React, { useState } from 'react';
import './SearchForm.scss';
import { Browser } from '../../types';

interface Props {
  onSearch: (data: { keyword: string; url: string; browser: Browser }) => void;
}

export const SearchForm: React.FC<Props> = ({ onSearch }) => {
  const [keyword, setKeyword] = useState('');
  const [url, setUrl] = useState('');
  const [browser, setBrowser] = useState<Browser>(Browser.All);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSearch({ keyword: keyword.trim(), url: url.trim(), browser });
  };

  return (
    <form className="search-form" onSubmit={handleSubmit}>
      <input
        type="text"
        placeholder="Keyword"
        value={keyword}
        onChange={(e) => setKeyword(e.target.value)}
        required
      />
      <input
        type="text"
        placeholder="URL"
        value={url}
        onChange={(e) => setUrl(e.target.value)}
        required
      />
      <label htmlFor="browser-select">Choose a browser:</label>
      <select id="browser-select" value={browser} onChange={(e) => setBrowser(e.target.value as Browser)}>
        <option value={Browser.All}>All</option>
        <option value={Browser.Google}>Google</option>
        <option value={Browser.Bing}>Bing</option>
      </select>
      <button type="submit">Search</button>
    </form>
  );
};