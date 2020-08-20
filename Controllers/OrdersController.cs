using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using System;
using System.Collections.Generic;
using feedme_backend.Models;

namespace feedme_backend.Controllers
{
    public class OrdersController : Controller
    {
        private FeedmeEntities db = new FeedmeEntities();

        public OrdersController()
        {
            db.Configuration.LazyLoadingEnabled = false;
        }

        // GET: GetOrdersByRes
        public ActionResult GetOrderByRes(int resId)
        {
            var orders = from x in db.Orders
                         where x.RestaurantFK == resId
                         select x;

            return Json(orders, JsonRequestBehavior.AllowGet);
        }

        // GET: GetOrdersByUser
        public ActionResult GetOrderByUser(int resId)
        {
            var orders = from x in db.Orders
                         where x.UserFK == resId
                         select x;

            return Json(orders, JsonRequestBehavior.AllowGet);
        }

        // POST: GiveRating
        [HttpPost]
        public ActionResult GiveRating(int orderId, int star, string comments)
        {
            var order = from x in db.Orders
                        where x.ID == orderId
                        select x;

            if (order.Any() && order.First().Ratings.Any())
            {
                var orderToRate = order.FirstOrDefault();
                var rating = new Rating { OrderFK = orderToRate.ID, UserFK = orderToRate.UserFK, Stars = star, Comments = comments, Timestamp = DateTime.Now };
                db.Ratings.Add(rating);
                db.SaveChanges();

                return Json(rating);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // POST: PlaceOrder
        [HttpPost]
        public ActionResult PlaceOrder(OrderPost orderPost)
        {
            var order = new Order
            {
                RestaurantFK = orderPost.ResId,
                UserFK = orderPost.UserId,
                Status = "Pending Confirmation",
                TimestampC = DateTime.Now,
                TimestampE = DateTime.Now
            };

            var order_add = db.Orders.Add(order);

            decimal total = 0;

            foreach (var orderItem in orderPost.OrderItems)
            {
                orderItem.OrderFK = order.ID;
                db.OrderItems.Add(orderItem);
                var price = (from x in db.Items
                             where x.ID == orderItem.ItemFK
                             select x.Price).FirstOrDefault();
                total += price * orderItem.Quantity;
            }

            order_add.OrderTotal = total;
            db.SaveChanges();

            return Json(order.ID);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}