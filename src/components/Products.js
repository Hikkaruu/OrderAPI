import '../App.css';
import axios from 'axios';
import React, { useEffect, useState } from 'react';

function Products() {
  const [data, setData] = useState([]);
  const [sortOrder, setSortOrder] = useState('asc');
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [categories, setCategories] = useState([]);
  const [postFormData, setPostFormData] = useState({
        name: '',
        price: '',
        weight: '',
        description: '',
        qty: '',
        categoryId: '' 
  });
  const [responseData, setResponseData] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await axios.get('https://localhost:7204/api/Product');
        setData(response.data);
      } catch (error) {
        console.error('Error fetching data:', error);
      }
    };
  
    fetchData();
  }, []);

  useEffect(() => {
    const fetchData = async () => {
      try {   
        let myGet = `https://localhost:7204/api/Product/${selectedCategory}/Category`;
        if (selectedCategory === 'all') {
          myGet = 'https://localhost:7204/api/Product'
        }
        let response = await axios.get(myGet);
        setData(response.data);
      } catch (error) {
        console.error('Error fetching data:', error);
        if (error.response && error.response.status === 404){
           setData([]);
         }
      }
    };

    fetchData();
  }, [selectedCategory]);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await axios.get('https://localhost:7204/api/Category');
        setCategories(response.data);
      } catch (error) {
        console.error('Error fetching categories:', error);
      }
    };

    fetchCategories();
  }, []);

  const sortByField = (field) => {
    const sortedData = [...data];
    if (sortOrder === 'asc') {
      sortedData.sort((a, b) => a[field] - b[field]);
      setSortOrder('desc');
    } else {
      sortedData.sort((a, b) => b[field] - a[field]);
      setSortOrder('asc');
    }
    setData(sortedData);
  };

  const handleCategoryChange = async (event) => {
    const category = event.target.value;
    setSelectedCategory(category);
  
    try {
      if (category === 'all') {
        const response = await axios.get('https://localhost:7204/api/Product');
        setData(response.data);
      } 
    } catch (error) {
      console.error('Error fetching data:', error);
    }
  };

  const addProduct = (e) => {
    e.preventDefault();
    axios.post(`https://localhost:7204/api/Product?categoryId=${postFormData.categoryId}`, postFormData)
      .then(response => {
        setResponseData(response.data);
      })
      .catch(error => {
        console.error('Error:', error);
      });
  };

  const postFormChange = (e) => {
    const { name, value } = e.target;
    setPostFormData({
      ...postFormData,
      [name]: value
    });
  };
  
  return (
    <div className="App">
      <div>
        <h2>Data from API:</h2>
        <div>
          <label htmlFor="category">Select Category:</label>
          <select id="category" value={selectedCategory} onChange={handleCategoryChange}>
            <option value="all">All</option>
            {categories.map(category => (
              <option key={category.id} value={category.id}>{category.name}</option>
            ))}
          </select>
        </div>
        <table>
          <thead>
            <tr>
              <th onClick={() => sortByField('name')}>Name {sortOrder === 'asc' ? '▲' : '▼'}</th>
              <th onClick={() => sortByField('price')}>Price {sortOrder === 'asc' ? '▲' : '▼'}</th>
              <th onClick={() => sortByField('weight')}>Weight {sortOrder === 'asc' ? '▲' : '▼'}</th>
              <th onClick={() => sortByField('description')}>Description {sortOrder === 'asc' ? '▲' : '▼'}</th>
              <th onClick={() => sortByField('qty')}>Qty {sortOrder === 'asc' ? '▲' : '▼'}</th>
            </tr>
          </thead>
          <tbody>
            {data.length > 0 ? (
                data.map(item => (
                    <tr key={item.id}>
                        <td>{item.name}</td>
                        <td>{item.price}</td>
                        <td>{item.weight}</td>
                        <td>{item.description}</td>
                        <td>{item.qty}</td>
                    </tr>
                    ))
                ) : 
                (
                    <tr>
                        <td colSpan="5">There are no products of the selected category.</td>
                    </tr>
                )}
        </tbody>
        </table>
        <form onSubmit={addProduct}>
            <h3>Add product</h3>
            <div>
                <label>name:</label>
                    <input 
                        type="text" 
                        name="name"
                        value={postFormData.name} 
                        onChange={postFormChange} 
                    />
            </div>
            <div>
                <label>price:</label>
                    <input 
                        type="number" 
                        name="price"
                        value={postFormData.price} 
                        onChange={postFormChange} 
                    />
            </div>
            <div>
                <label>weight:</label>
                    <input 
                        type="number" 
                        name="weight"
                        value={postFormData.weight} 
                        onChange={postFormChange} 
                    />
            </div>
            <div>
                <label>description:</label>
                    <input 
                        type="text" 
                        name="description"
                        value={postFormData.description} 
                        onChange={postFormChange} 
                    />
            </div>
            <div>
                <label>quantity:</label>
                    <input 
                        type="number" 
                        name="qty"
                        value={postFormData.qty} 
                        onChange={postFormChange} 
                    />
            </div>
            <div>
              <label>Category:</label>
              <select
                name="categoryId"
                value={postFormData.categoryId}
                onChange={postFormChange}
              >
                <option value="">Select Category</option>
                {categories.map(category => (
                  <option key={category.id} value={category.id}>
                    {category.name}
                  </option>
                ))}
              </select>
            </div>
            <button type="submit">Add Product</button>
        </form>
        {responseData && (
        <div>
          <h2>Product: {postFormData.name} </h2>
          <pre>{JSON.stringify(responseData, null, 2)}</pre>
        </div>
      )}
      </div>
    </div>
  );
}

export default Products;
