using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System.Data;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsEntityManagerController : BpmsAdminApiControlBase
    {
        // GET: /BpmsTableManager/
        public object GetList([System.Web.Http.FromUri] EntityIndexSearchDTO indexSearchVM)
        {
            //base.SetMenuIndex(AdminMenuIndex.EntityManagerIndex);
            using (EntityDefService entityDefService = new EntityDefService())
            {
                indexSearchVM.Update(entityDefService.GetList("", indexSearchVM.Name, null, indexSearchVM.GetPagingProperties).Select(c => new EntityDefDTO(c)).ToList());
                return indexSearchVM;
            }
        }

        [HttpGet]
        public object GetAddEdit(Guid? ID = null)
        {
            using (EntityDefService entityDefService = new EntityDefService())
            {
                EntityDefDTO entityDef = new EntityDefDTO(ID.HasValue ? entityDefService.GetInfo(ID.Value) : null);
                entityDef.Relations.ForEach(c =>
                {
                    c.GetEntityDefProperties = c.EntityDefID != Guid.Empty ? entityDefService.GetInfo(c.EntityDefID).AllProperties : new List<EntityPropertyModel>();
                });

                List<sysBpmsEntityDef> AllPublishedEntityDefs = entityDefService.GetList(string.Empty, string.Empty, true);
                return Json(new
                {
                    Model = entityDef,
                    DbTypes = EnumObjHelper.GetEnumList<EntityPropertyModel.e_dbType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)),
                    RelationEntityDefs = AllPublishedEntityDefs.Select(c => new QueryModel(c.ID.ToString(), c.Name)),
                    RelationProperties = entityDef.AllProperties
                });
            }
        }

        [HttpPost]
        public object PostAddEdit(PostAddEditEntityDefDTO postAddEdit)
        {
            using (EntityDefService entityDefService = new EntityDefService())
            {
                sysBpmsEntityDef entityDef = postAddEdit.EntityDefDTO.ID != Guid.Empty ? entityDefService.GetInfo(postAddEdit.EntityDefDTO.ID) : new sysBpmsEntityDef();

                if (postAddEdit.listProperties != null)
                {
                    foreach (var Item in postAddEdit.listProperties)
                    {
                        if (string.IsNullOrWhiteSpace(Item.ID))
                            Item.ID = Guid.NewGuid().ToString();
                        Item.IsActive = true;
                        postAddEdit.EntityDefDTO.Properties.Add(Item);
                    }
                }
                if (postAddEdit.listRelations != null)
                {
                    foreach (var Item in postAddEdit.listRelations)
                    {
                        Item.ID = string.IsNullOrWhiteSpace(Item.ID) ? Guid.NewGuid().ToString() : Item.ID;
                        postAddEdit.EntityDefDTO.Relations.Add(Item);
                    }
                }

                ResultOperation resultOperation = entityDef.Update(postAddEdit.EntityDefDTO.DisplayName, postAddEdit.EntityDefDTO.Name, postAddEdit.EntityDefDTO.TableName, postAddEdit.EntityDefDTO.DesignXML, true, postAddEdit.EntityDefDTO.Properties, postAddEdit.EntityDefDTO.Relations);
                if (resultOperation.IsSuccess)
                {
                    if (entityDef.ID != Guid.Empty)
                        resultOperation = entityDefService.Update(entityDef);
                    else
                        resultOperation = entityDefService.Add(entityDef);

                    if (resultOperation.IsSuccess)
                    {
                        return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success, entityDef.ID);
                    }
                    else
                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                }
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

            }
        }

        public object ExecuteQuery(Guid entityId, string query = "")
        {
            using (EntityDefService entityDefService = new EntityDefService())
            {
                using (DataBaseQueryService dataBaseQueryService = new DataBaseQueryService())
                {
                    try
                    {
                        sysBpmsEntityDef entityDef = entityDefService.GetInfo(entityId);
                        query = string.IsNullOrWhiteSpace(query) ? $" select top(200) * from {entityDef.FormattedTableName} " : query;
                        DataTable data = dataBaseQueryService.GetBySqlQuery(query, true, null);

                        return Json(new { Data = data, EntityId = entityId, Query = query });
                    }
                    catch (Exception ex)
                    {
                        return new PostMethodMessage(ex.ToStringObj(), DisplayMessageType.error);
                    }

                }
            }
        }

        [HttpGet]
        public object GetInActive(Guid ID)
        {
            ResultOperation result = new EntityDefService().InActive(ID);
            if (result.IsSuccess)
                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            else
                return new PostMethodMessage(result.GetErrors(), DisplayMessageType.success);
        }

        [HttpGet]
        public object GetActive(Guid ID)
        {
            ResultOperation result = new EntityDefService().Active(ID);
            if (result.IsSuccess)
                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            else
                return new PostMethodMessage(result.GetErrors(), DisplayMessageType.success);
        }


        [HttpDelete]
        public object Delete(Guid ID)
        {
            if (ID != Guid.Empty)
            {
                ResultOperation result = new EntityDefService().Delete(ID);
                if (result.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(result.GetErrors(), DisplayMessageType.error);
            }
            else
            {
                return new PostMethodMessage(SharedLang.Get("NotFound.Text"), DisplayMessageType.error);
            }
        }
        [HttpGet]
        public object GetRelationProperties(Guid EntityDefId)
        {
            return new EntityDefService().GetInfo(EntityDefId).AllProperties;
        }


    }
}