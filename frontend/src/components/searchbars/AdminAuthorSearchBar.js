import React, { useState, useCallback } from 'react';
import axios from 'axios';
import {
  TextField,
  Box,
  IconButton,
} from '@mui/material';

const AdminAuthorSearchBar = ({ onSearchResult }) => {
  const [searchTerm, setSearchTerm] = useState('');

  const handleTextChange = useCallback((e) => {
    const value = e.target.value;
    setSearchTerm(value);
  }, []);

  const handleSearch = useCallback(async () => {
    try {
      const params = new URLSearchParams();
      if (searchTerm) params.append('searchTerm', searchTerm);
      params.append('pageNo', 1);
      params.append('itemsPerPage', 8);
      
      const response = await axios.get(`${process.env.REACT_APP_API_URL}/authors/filtred-list?${params.toString()}`);
      onSearchResult(response.data.items, response.data.totalPages);
    } catch (error) {
      console.error('Error searching authors:', error);
      onSearchResult([], 1);
    }
  }, [searchTerm, onSearchResult]);

  return (
    <Box sx={{ 
      width: '100%', 
      py: 2,
      display: 'flex',
      flexWrap: 'wrap',
      gap: 2,
      alignItems: 'center',
    }}>
      <TextField
        sx={{ flex: '1 1 400px' }}
        label="Search Authors"
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

export default React.memo(AdminAuthorSearchBar);
