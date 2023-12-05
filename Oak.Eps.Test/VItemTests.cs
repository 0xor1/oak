// using Common.Shared;
// using Oak.Api.VItem;
//
// namespace Oak.Eps.Test;
//
// public class VItemTests : TestBase
// {
//     [Fact]
//     public async void Create_Success()
//     {
//         var (ali, bob, cat, dan, anon, org) = await Setup();
//         var aliSes = await ali.Auth.GetSession();
//         var tt = await CreateTaskTree(ali, org.Id);
//         var res = await ali.VItem.Create(
//             new(org.Id, tt.P.Id, tt.E.Id, VItemType.Time, 120ul, 60ul, "yolo")
//         );
//         await tt.Refresh();
//         var vi = new VItem(
//             org.Id,
//             tt.P.Id,
//             tt.E.Id,
//             VItemType.Time,
//             res.Item.Id,
//             aliSes.Id,
//             res.Item.CreatedOn,
//             60ul,
//             "yolo"
//         );
//         Assert.Equal(res.Item, vi);
//         Assert.Equal(tt.E, res.Task);
//         Assert.Equal(60ul, tt.P.TimeSubInc);
//         Assert.Equal(aliSes.Id, vi.CreatedBy);
//     }
//
//     [Fact]
//     public async void Update_Success()
//     {
//         var (ali, bob, cat, dan, anon, org) = await Setup();
//         var tt = await CreateTaskTree(ali, org.Id);
//         var res = await ali.VItem.Create(
//             new(org.Id, tt.P.Id, tt.E.Id, VItemType.Time, 120ul, 60ul, "yolo")
//         );
//         var vi = res.Item;
//         res = await ali.VItem.Update(
//             new(vi.Org, vi.Project, vi.Task, vi.Type, vi.Id, 10ul, "nolo")
//         );
//         await tt.Refresh();
//         Assert.Equal(10ul, res.Item.Inc);
//         Assert.Equal("nolo", res.Item.Note);
//         Assert.Equal(tt.E, res.Task);
//         Assert.Equal(10ul, tt.P.TimeSubInc);
//
//         res = await ali.VItem.Update(
//             new(vi.Org, vi.Project, vi.Task, vi.Type, vi.Id, 60ul, "solo")
//         );
//         await tt.Refresh();
//         Assert.Equal(60ul, res.Item.Inc);
//         Assert.Equal("solo", res.Item.Note);
//         Assert.Equal(tt.E, res.Task);
//         Assert.Equal(60ul, tt.P.TimeSubInc);
//
//         res = await ali.VItem.Create(
//             new(org.Id, tt.P.Id, tt.E.Id, VItemType.Cost, 120ul, 60ul, "yolo")
//         );
//         vi = res.Item;
//         res = await ali.VItem.Update(
//             new(vi.Org, vi.Project, vi.Task, vi.Type, vi.Id, 10ul, "nolo")
//         );
//         await tt.Refresh();
//         Assert.Equal(120ul, res.Task.CostEst);
//         Assert.Equal(10ul, res.Item.Inc);
//         Assert.Equal("nolo", res.Item.Note);
//         Assert.Equal(tt.E, res.Task);
//         Assert.Equal(10ul, tt.P.CostSubInc);
//
//         res = await ali.VItem.Update(
//             new(vi.Org, vi.Project, vi.Task, vi.Type, vi.Id, 60ul, "solo")
//         );
//         await tt.Refresh();
//         Assert.Equal(60ul, res.Item.Inc);
//         Assert.Equal("solo", res.Item.Note);
//         Assert.Equal(tt.E, res.Task);
//         Assert.Equal(60ul, tt.P.CostSubInc);
//
//         res = await ali.VItem.Create(
//             new(org.Id, tt.P.Id, tt.E.Id, VItemType.Cost, null, 60ul, "yolo")
//         );
//         Assert.Equal(120ul, res.Task.CostEst);
//         Assert.Equal(120ul, res.Task.CostInc);
//     }
//
//     [Fact]
//     public async void Delete_Success()
//     {
//         var (ali, bob, cat, dan, anon, org) = await Setup();
//         var tt = await CreateTaskTree(ali, org.Id);
//         var res = await ali.VItem.Create(
//             new(org.Id, tt.P.Id, tt.E.Id, VItemType.Time, null, 60ul, "yolo")
//         );
//         var e = await ali.VItem.Delete(new(org.Id, tt.P.Id, tt.E.Id, res.Item.Type, res.Item.Id));
//         Assert.Equal(tt.E, e);
//         Assert.Equal(0ul, tt.P.TimeSubInc);
//         res = await ali.VItem.Create(
//             new(org.Id, tt.P.Id, tt.E.Id, VItemType.Cost, null, 60ul, "yolo")
//         );
//         e = await ali.VItem.Delete(new(org.Id, tt.P.Id, tt.E.Id, res.Item.Type, res.Item.Id));
//         Assert.Equal(tt.E, e);
//         Assert.Equal(0ul, tt.P.CostSubInc);
//     }
//
//     [Fact]
//     public async void Get_Success()
//     {
//         var (ali, bob, cat, dan, anon, org) = await Setup();
//         var aliSes = await ali.Auth.GetSession();
//         var tt = await CreateTaskTree(ali, org.Id);
//         var res = await ali.VItem.Create(
//             new(org.Id, tt.P.Id, tt.E.Id, VItemType.Time, null, 1ul, "a")
//         );
//         var a = res.Item;
//         res = await ali.VItem.Create(new(org.Id, tt.P.Id, tt.E.Id, VItemType.Time, null, 2ul, "b"));
//         var b = res.Item;
//
//         var getRes = await ali.VItem.Get(
//             new(
//                 a.Org,
//                 a.Project,
//                 a.Type,
//                 a.Task,
//                 new(
//                     DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-2)),
//                     DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(2))
//                 ),
//                 aliSes.Id
//             )
//         );
//         var @is = getRes.Set;
//         Assert.Equal(2, @is.Count);
//         Assert.Equal(b, @is[0]);
//         Assert.Equal(a, @is[1]);
//
//         getRes = await ali.VItem.Get(
//             new(
//                 a.Org,
//                 a.Project,
//                 a.Type,
//                 a.Task,
//                 new(
//                     DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-2)),
//                     DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(2))
//                 ),
//                 aliSes.Id,
//                 Asc: true
//             )
//         );
//         @is = getRes.Set;
//         Assert.Equal(2, @is.Count);
//         Assert.Equal(a, @is[0]);
//         Assert.Equal(b, @is[1]);
//
//         getRes = await ali.VItem.Get(
//             new(
//                 a.Org,
//                 a.Project,
//                 a.Type,
//                 a.Task,
//                 new(
//                     DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-2)),
//                     DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(2))
//                 ),
//                 aliSes.Id,
//                 b.Id
//             )
//         );
//         @is = getRes.Set;
//         Assert.Equal(1, @is.Count);
//         Assert.Equal(a, @is[0]);
//
//         getRes = await ali.VItem.Get(
//             new(
//                 a.Org,
//                 a.Project,
//                 a.Type,
//                 a.Task,
//                 new(
//                     DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-2)),
//                     DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(2))
//                 ),
//                 aliSes.Id,
//                 a.Id,
//                 true
//             )
//         );
//         @is = getRes.Set;
//         Assert.Equal(1, @is.Count);
//         Assert.Equal(b, @is[0]);
//     }
// }
