using AutoMapper;
using FluentValidation;
using MongoDB.Driver;
using OrderService.BusinessLayer.Dtos;
using OrderService.BusinessLayer.HttpClients;
using OrderService.BusinessLayer.ServiceContracts;
using OrderService.DataAccessLayer.RepositoryContracts;
using OrderService.RepositoryLayer.Entities;

namespace OrderService.BusinessLayer.Services
{
    public class OrderServices : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<OrderAddRequest> _orderAddRequestValidator;
        private readonly IValidator<OrderUpdateRequest> _orderUpdateRequestValidator;
        private readonly IValidator<OrderItemsAddRequest> _orderItemsAddRequestValidator;
        private readonly IValidator<OrderItemsUpdateRequest> _orderItemsUpdateRequestValidator;
        private readonly UserMicroservicesClient _userMicrosoftClient;
        private readonly ProductMicroservicesClient _productMicroservicesClient;

        public OrderServices(IOrderRepository orderRepository,
            IMapper mapper,
            IValidator<OrderAddRequest> orderAddRequestValidator,
            IValidator<OrderUpdateRequest> orderUpdateRequestValidator,
            IValidator<OrderItemsAddRequest> orderItemsAddRequestValidator,
            IValidator<OrderItemsUpdateRequest> orderItemsUpdateRequestValidator,
            UserMicroservicesClient userMicrosoftClient,
            ProductMicroservicesClient productMicroservicesClient)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _orderAddRequestValidator = orderAddRequestValidator;
            _orderUpdateRequestValidator = orderUpdateRequestValidator;
            _orderItemsAddRequestValidator = orderItemsAddRequestValidator;
            _orderItemsUpdateRequestValidator = orderItemsUpdateRequestValidator;
            _userMicrosoftClient = userMicrosoftClient;
            _productMicroservicesClient = productMicroservicesClient;
        }

        public async Task<OrderResponse> CreateAsync(OrderAddRequest? orderRequest)
        {
            if (orderRequest == null)
            {
                throw new ArgumentNullException(nameof(orderRequest));
            }

            var validationAddRequestResult = _orderAddRequestValidator.Validate(orderRequest);
            if (!validationAddRequestResult.IsValid)
            {
                var errors = string.Join(", ", validationAddRequestResult.Errors.Select(x => x.ErrorMessage));
                throw new ValidationException(errors);
            }
            UserDto? user = await _userMicrosoftClient.GetUserByID(orderRequest.UserID);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            var orderItemsErrors = new List<string>();
            foreach (var item in orderRequest.OrderItems)
            {
                var validationOrderItemsAddRequestResult = _orderItemsAddRequestValidator.Validate(item);
                if (!validationOrderItemsAddRequestResult.IsValid)
                {
                    orderItemsErrors.AddRange(validationOrderItemsAddRequestResult.Errors.Select(x => x.ErrorMessage));
                }
                ProductDto? product = await _productMicroservicesClient.GetProductByID(item.ProductID);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {item.ProductID} not found");
                }
            }

            if (orderItemsErrors.Any())
            {
                throw new ValidationException("Order items validation failed: " + string.Join(", ", orderItemsErrors));
            }
          
            var order = _mapper.Map<Order>(orderRequest);
            await _orderRepository.CreateAsync(order);
            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<bool> DeleteAsync(Guid orderId)
        {
            var filter = Builders<Order>.Filter.Eq(x => x.OrderID, orderId);
            var existingOrder = await _orderRepository.GetByAsync(filter);
            if (existingOrder == null)
            {
                return false;
            }
            return await _orderRepository.DeleteAsync(orderId);
        }

        public async Task<IEnumerable<OrderResponse>> GetAllAsync(FilterDefinition<Order>? filter = null)
        {
            var orders = await _orderRepository.GetAllAsync(filter);
            if (orders == null || !orders.Any())
            {
                return Enumerable.Empty<OrderResponse>();
            }
            return _mapper.Map<IEnumerable<OrderResponse>>(orders);
        }

        public async Task<OrderResponse> GetByAsync(FilterDefinition<Order> filter)
        {
            var order = await _orderRepository.GetByAsync(filter);
            if (order == null)
            {
                return null;
            }
            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<OrderResponse> UpdateAsync(OrderUpdateRequest? orderRequest)
        {
            if (orderRequest == null)
            {
                throw new ArgumentNullException(nameof(orderRequest));
            }

            var validationUpdateRequestResult = _orderUpdateRequestValidator.Validate(orderRequest);
            if (!validationUpdateRequestResult.IsValid)
            {
                var errors = string.Join(", ", validationUpdateRequestResult.Errors.Select(x => x.ErrorMessage));
                throw new ValidationException(errors);
            }

            var orderItemsErrors = new List<string>();
            foreach (var item in orderRequest.OrderItems)
            {
                var validationOrderItemsUpdateRequestResult = _orderItemsUpdateRequestValidator.Validate(item);
                if (!validationOrderItemsUpdateRequestResult.IsValid)
                {
                    orderItemsErrors.AddRange(validationOrderItemsUpdateRequestResult.Errors.Select(x => x.ErrorMessage));
                }
                ProductDto? product = await _productMicroservicesClient.GetProductByID(item.ProductID);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {item.ProductID} not found");
                }
            }

            if (orderItemsErrors.Any())
            {
                throw new ValidationException("Order items validation failed: " + string.Join(", ", orderItemsErrors));
            }
            UserDto? user = await _userMicrosoftClient.GetUserByID(orderRequest.UserID);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            var filter = Builders<Order>.Filter.Eq(x => x.OrderID, orderRequest.OrderID);
            var existingOrder = await _orderRepository.GetByAsync(filter);
            if (existingOrder == null)
            {
                throw new KeyNotFoundException("Order not found");
            }

            _mapper.Map(orderRequest, existingOrder);
            await _orderRepository.UpdateAsync(existingOrder);
            return _mapper.Map<OrderResponse>(existingOrder);
        }
    }
}
