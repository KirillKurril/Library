import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { useSearchParams } from 'react-router-dom';
import axios from 'axios';
import {
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Checkbox,
  FormControlLabel,
  Box,
} from '@mui/material';

const BookSearchBar = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const [searchTerm, setSearchTerm] = useState(searchParams.get('searchTerm') || '');
  const [selectedAuthor, setSelectedAuthor] = useState(searchParams.get('AuthorId') || '');
  const [selectedGenre, setSelectedGenre] = useState(searchParams.get('genreId') || '');
  const [isIsbnSearch, setIsIsbnSearch] = useState(false);
  const [authors, setAuthors] = useState([]);
  const [genres, setGenres] = useState([]);

  useEffect(() => {
    const fetchFilters = async () => {
      try {
        const [authorsResponse, genresResponse] = await Promise.all([
          axios.get(`${process.env.REACT_APP_API_URL}/authors/list`),
          axios.get(`${process.env.REACT_APP_API_URL}/genres/list`)
        ]);
        setAuthors(authorsResponse.data);
        setGenres(genresResponse.data);
      } catch (error) {
        console.error('Error fetching filters:', error);
      }
    };

    fetchFilters();
  }, []);

  const updateSearchParams = useCallback((params) => {
    const newSearchParams = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => {
      if (value) newSearchParams.set(key, value);
    });
    setSearchParams(newSearchParams);
  }, [setSearchParams]);

  const handleTextChange = useCallback((e) => {
    const value = e.target.value;
    setSearchTerm(value);
    if (!isIsbnSearch) {
      const timeoutId = setTimeout(() => {
        updateSearchParams({
          searchTerm: value,
          AuthorId: selectedAuthor,
          genreId: selectedGenre
        });
      }, 300);
      return () => clearTimeout(timeoutId);
    }
  }, [isIsbnSearch, selectedAuthor, selectedGenre, updateSearchParams]);

  const handleAuthorChange = useCallback((e) => {
    const value = e.target.value;
    setSelectedAuthor(value);
    updateSearchParams({
      searchTerm,
      AuthorId: value,
      genreId: selectedGenre
    });
  }, [searchTerm, selectedGenre, updateSearchParams]);

  const handleGenreChange = useCallback((e) => {
    const value = e.target.value;
    setSelectedGenre(value);
    updateSearchParams({
      searchTerm,
      AuthorId: selectedAuthor,
      genreId: value
    });
  }, [searchTerm, selectedAuthor, updateSearchParams]);

  const handleIsbnCheckboxChange = useCallback((e) => {
    const checked = e.target.checked;
    setIsIsbnSearch(checked);
    if (checked) {
      updateSearchParams({});
    } else {
      updateSearchParams({
        searchTerm,
        AuthorId: selectedAuthor,
        genreId: selectedGenre
      });
    }
  }, [searchTerm, selectedAuthor, selectedGenre, updateSearchParams]);

  const handleIsbnSearch = useCallback(async () => {
    if (isIsbnSearch && searchTerm) {
      try {
        const response = await axios.get(`${process.env.REACT_APP_API_URL}/books/${searchTerm}`);
        if (response.data) {
          setSelectedAuthor('');
          setSelectedGenre('');
          updateSearchParams({ isbn: searchTerm });
        }
      } catch (error) {
        console.error('Error searching by ISBN:', error);
      }
    }
  }, [isIsbnSearch, searchTerm, updateSearchParams]);

  const authorItems = useMemo(() => (
    authors.map((author) => (
      <MenuItem key={author.id} value={author.id}>
        {`${author.name} ${author.surname}`}
      </MenuItem>
    ))
  ), [authors]);

  const genreItems = useMemo(() => (
    genres.map((genre) => (
      <MenuItem key={genre.id} value={genre.id}>
        {genre.name}
      </MenuItem>
    ))
  ), [genres]);

  return (
    <Box sx={{ display: 'flex', gap: 2, alignItems: 'center', p: 2, flexWrap: 'wrap' }}>
      <TextField
        label={isIsbnSearch ? "ISBN Search" : "Search Books"}
        variant="outlined"
        value={searchTerm}
        onChange={handleTextChange}
        onKeyPress={(e) => {
          if (e.key === 'Enter' && isIsbnSearch) {
            handleIsbnSearch();
          }
        }}
        size="small"
        sx={{ minWidth: 200 }}
      />

      <FormControl size="small" sx={{ minWidth: 200 }}>
        <InputLabel>Author</InputLabel>
        <Select
          value={selectedAuthor}
          onChange={handleAuthorChange}
          label="Author"
          disabled={isIsbnSearch}
        >
          <MenuItem value="">
            <em>All Authors</em>
          </MenuItem>
          {authorItems}
        </Select>
      </FormControl>

      <FormControl size="small" sx={{ minWidth: 200 }}>
        <InputLabel>Genre</InputLabel>
        <Select
          value={selectedGenre}
          onChange={handleGenreChange}
          label="Genre"
          disabled={isIsbnSearch}
        >
          <MenuItem value="">
            <em>All Genres</em>
          </MenuItem>
          {genreItems}
        </Select>
      </FormControl>

      <FormControlLabel
        control={
          <Checkbox
            checked={isIsbnSearch}
            onChange={handleIsbnCheckboxChange}
          />
        }
        label="Search by ISBN"
      />
    </Box>
  );
};

export default React.memo(BookSearchBar);