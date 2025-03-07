import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
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
  IconButton,
} from '@mui/material';

const BookSearchBar = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();
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
  }, []);

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
          navigate(`/books/${response.data.id}`);
        }
      } catch (error) {
        console.error('Error searching by ISBN:', error);
      }
    }
  }, [isIsbnSearch, searchTerm, navigate]);

  const handleSearch = useCallback(() => {
    if (!isIsbnSearch) {
      updateSearchParams({
        searchTerm,
        AuthorId: selectedAuthor,
        genreId: selectedGenre
      });
    } else if (searchTerm) {
      handleIsbnSearch();
    }
  }, [isIsbnSearch, searchTerm, selectedAuthor, selectedGenre, updateSearchParams, handleIsbnSearch]);

  return (
    <Box sx={{ 
      width: '100%', 
      py: 2,
      mb: 2,
      display: 'flex',
      flexWrap: 'wrap',
      gap: 2,
      alignItems: 'center',
      '& .MuiFormControl-root': { 
        minWidth: '200px',
      },
      '& .MuiSelect-select': { 
        whiteSpace: 'normal' 
      }
    }}>
      <TextField
        sx={{ flex: '2 1 400px' }}
        label={isIsbnSearch ? "Enter ISBN" : "Search Books"}
        variant="outlined"
        value={searchTerm}
        onChange={handleTextChange}
        onKeyPress={(e) => {
          if (e.key === 'Enter') {
            if (isIsbnSearch) {
              handleIsbnSearch();
            } else {
              handleSearch();
            }
          }
        }}
        size="small"
      />
      
      <FormControl size="small" sx={{ flex: '1 1 200px' }}>
        <InputLabel>Author</InputLabel>
        <Select
          value={selectedAuthor}
          onChange={handleAuthorChange}
          label="Author"
          disabled={isIsbnSearch}
          MenuProps={{
            PaperProps: {
              style: { maxHeight: 300 }
            }
          }}
        >
          <MenuItem value="">
            <em>All Authors</em>
          </MenuItem>
          {authors.map((author) => (
            <MenuItem key={author.id} value={author.id}>
              {`${author.name} ${author.surname}`}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      <FormControl size="small" sx={{ flex: '1 1 200px' }}>
        <InputLabel>Genre</InputLabel>
        <Select
          value={selectedGenre}
          onChange={handleGenreChange}
          label="Genre"
          disabled={isIsbnSearch}
          MenuProps={{
            PaperProps: {
              style: { maxHeight: 300 }
            }
          }}
        >
          <MenuItem value="">
            <em>All Genres</em>
          </MenuItem>
          {genres.map((genre) => (
            <MenuItem key={genre.id} value={genre.id}>
              {genre.name}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      <FormControlLabel
        control={
          <Checkbox
            checked={isIsbnSearch}
            onChange={handleIsbnCheckboxChange}
            size="small"
          />
        }
        label="Search by ISBN"
        sx={{ 
          flex: '0 0 auto',
          m: 0,
          '& .MuiFormControlLabel-label': { 
            whiteSpace: 'nowrap',
            fontSize: '0.875rem'
          }
        }}
      />

      <IconButton 
        onClick={isIsbnSearch ? handleIsbnSearch : handleSearch}
        sx={{ 
          flex: '0 0 auto',
          bgcolor: 'primary.main', 
          color: 'white',
          '&:hover': {
            bgcolor: 'primary.dark',
          }
        }}
        size="small"
      >
        <i className="fas fa-search"></i>
      </IconButton>
    </Box>
  );
};

export default React.memo(BookSearchBar);