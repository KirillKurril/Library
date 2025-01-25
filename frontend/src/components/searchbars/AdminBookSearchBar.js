import React, { useState, useEffect, useCallback } from 'react';
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

const AdminBookSearchBar = ({ onSearchResult }) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedAuthor, setSelectedAuthor] = useState('');
  const [selectedGenre, setSelectedGenre] = useState('');
  const [isIsbnSearch, setIsIsbnSearch] = useState(false);
  const [authors, setAuthors] = useState([]);
  const [genres, setGenres] = useState([]);
  const [previousFilters, setPreviousFilters] = useState({
    author: '',
    genre: ''
  });

  useEffect(() => {
    const fetchFilters = async () => {
      try {
        const [authorsResponse, genresResponse] = await Promise.all([
          axios.get(`${process.env.REACT_APP_API_URL}/authors/for-filtration`),
          axios.get(`${process.env.REACT_APP_API_URL}/genres/list`)
        ]);
        console.log('Authors data:', authorsResponse.data);
        setAuthors(authorsResponse.data);
        setGenres(genresResponse.data);
      } catch (error) {
        console.error('Error fetching filters:', error);
      }
    };

    fetchFilters();
  }, []);

  const handleTextChange = useCallback((e) => {
    const value = e.target.value;
    setSearchTerm(value);
  }, []);

  const handleAuthorChange = useCallback((e) => {
    const value = e.target.value;
    setSelectedAuthor(value);
  }, []);

  const handleGenreChange = useCallback((e) => {
    const value = e.target.value;
    setSelectedGenre(value);
  }, []);

  const handleIsbnCheckboxChange = useCallback((e) => {
    const checked = e.target.checked;
    setIsIsbnSearch(checked);
    if (checked) {
      setPreviousFilters({
        author: selectedAuthor,
        genre: selectedGenre
      });
      setSelectedAuthor('');
      setSelectedGenre('');
    } else {
      setSelectedAuthor(previousFilters.author);
      setSelectedGenre(previousFilters.genre);
    }
  }, [selectedAuthor, selectedGenre, previousFilters]);

  const handleSearch = useCallback(async () => {
    if (!searchTerm && !selectedAuthor && !selectedGenre) return;

    try {
      if (isIsbnSearch) {
        const response = await axios.get(`${process.env.REACT_APP_API_URL}/books/${searchTerm}`);
        if (response.data) {
          onSearchResult([response.data]);
        }
      } else {
        const params = new URLSearchParams();
        if (searchTerm) params.append('searchTerm', searchTerm);
        if (selectedAuthor) params.append('AuthorId', selectedAuthor);
        if (selectedGenre) params.append('genreId', selectedGenre);
        params.append('pageNo', 1);
        params.append('itemsPerPage', 10);
        
        const response = await axios.get(`${process.env.REACT_APP_API_URL}/books/catalog?${params.toString()}`);
        onSearchResult(response.data.items);
      }
    } catch (error) {
      console.error('Error searching books:', error);
    }
  }, [isIsbnSearch, searchTerm, selectedAuthor, selectedGenre, onSearchResult]);

  return (
    <Box sx={{ 
      width: '100%', 
      py: 2,
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
            handleSearch();
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
              {author.name}
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
        onClick={handleSearch}
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

export default React.memo(AdminBookSearchBar);
