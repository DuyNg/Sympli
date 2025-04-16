import React, { useState } from 'react';
import './App.scss';
import { SearchForm } from './components/SearchForm/SearchForm';
import { ResultsTable } from './components/ResultsTable/ResultsTable';
import { SearchResult } from './models';
import { searchKeyword } from './api/searchApi';
import { Browser } from './types';

const App: React.FC = () => {
  const [results, setResults] = useState<SearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSearch = async (data: { keyword: string; url: string; browser: Browser }) => {
    setLoading(true);
    setError(null);
    setResults([]);
  
    try {
      if (data.browser === 'All') {
        const [googleResult, bingResult] = await Promise.all([
          searchKeyword({ ...data, browser: Browser.Google }),
          searchKeyword({ ...data, browser: Browser.Bing }),
        ]);
        setResults([googleResult, bingResult]);
      } else {
        const result = await searchKeyword(data);
        setResults([result]);
      }
    } catch (err) {
      setError('Failed to fetch results. Please try again.');
    } finally {
      setLoading(false);
    }
  };  

  return (
    <div className="container">
      <div className="title">Search Application</div>
      <SearchForm onSearch={handleSearch} />
      {loading && <p>Loading...</p>}
      {error && <p className="error-message">{error}</p>}
      <ResultsTable results={results} />
    </div>
  );
};

export default App;
